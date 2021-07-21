using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(NPC_Animator))]
public class NPC : MonoBehaviour, IPathUnit
{
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float turnSpeed;

    NPC_Animator animator;
    CharacterController controller;
    Queue<Vector3> path;
    Vector3 destination;
    Quaternion lookRotation;
    Furniture targetFurn;
    Item itemToBuy;
    bool waitingForPurchase;
    CostumerInteractionBuble buble;


    Action updateCurrentStates;
    Action<NPC> pathCompleted;

    const float destination_margin = 1 / 9f;

    public bool OnPath => path.Count > 0;
    Vector3 Position { get => transform.position; set => transform.position = value; }
    bool HasArrived => (Position - destination).sqrMagnitude < destination_margin;
    public Furniture TargetFurniture { get => targetFurn; set => targetFurn = value; }
    public Item ItemToBuy { get => itemToBuy; set => itemToBuy = value; }

    public CostumerInteractionBuble Buble => buble;


    protected virtual void Awake()
    {
        animator = GetComponent<NPC_Animator>();
        controller = GetComponent<CharacterController>();
        lookRotation = transform.rotation;
        destination = transform.position;
        pathCompletedTree = new CallbackTree<Action>();
        pathCompletedTree.Stack(PathCompleted);
        consumeNextNode = ConsumeNextNode;
        updatePathProgress = UpdatePosAndRot;
        //SetRandomDestination();
    }

    void Start()
    {
        GridController.GetNodeAt(Position).SetPathUnit(this);
    }

    protected virtual void Update()
    {
        updateCurrentStates?.Invoke();
    }



    public void TakeItem()
    {
        targetFurn.RemoveItem(itemToBuy);
        Destroy(itemToBuy.gameObject);
    }

    public void LeaveShop(Action<NPC> onShopLefted)
    {
        if (buble != null)
        {
            GridController.GetNodeAt(Position).SetWalkable(true);
            Countier.RemoveCostumerFromQueue(this);
            Destroy(buble.gameObject);
        }

        SetDestination(new Vector3(new System.Random().Next(15, 21), 0, -9), onShopLefted);
    }


    public void ChooseItem()
    {
        StartCoroutine(nameof(ChoosingItem));
    }

    public IEnumerator ChoosingItem()
    {
        // TODO: check for the furniture: it is still there?, destroyed? crowded?
        LookFurniture();

        //yield return new WaitForSeconds(new System.Random().Next(0, 3));
        yield return null;

        // TODO: what if there is no proper item to choose?
        if (GetItem())
            SetDestination(Countier.GetValidNearByNode().Position,
                             (npc) => { NPCController.Instance.NPC_ReadyToPurchase(this); });
        else
            LeaveShop(NPCController.Instance.NPC_LeftTheShop);
    }


    bool GetItem()
    {
        if (targetFurn.IsEmpty)
        {
            return false;
        }
        itemToBuy = targetFurn.GetRandomItem();
        itemToBuy.PlayFlashingAnim();
        return true;
    }

    public void WaitForPurchase()
    {
        waitingForPurchase = true;
        Countier.AddCostumerToQueue(this);
        CreateInteractionBuble();
        GridController.GetNodeAt(Position).SetWalkable(false);
    }

    void CreateInteractionBuble()
    {
        var buble = Instantiate(CostumerInteractionsMenu.Instance.BublePrefab);
        buble.transform.position = transform.position + Vector3.up * 3.4f;
        buble.SetIcon(itemToBuy.Icon);
        buble.Owner = this;
        this.buble = buble;
    }



    public void SetDestination(Vector3 destination, Action<NPC> pathCompletedCallback)
    {
        path = new Queue<Vector3>(PathFinder.FindPath(Position, destination));
        if (path.Count > 0 && GridController.GetNodeAt(path.Last()).Walkable == false)
        {
            List<Vector3> list = path.ToList();
            list.RemoveAt(list.Count - 1);
            path = new Queue<Vector3>(list);
        }
        animator.Walk();
        pathCompleted = pathCompletedCallback;
        if (pathCompletedTree.Length > 1)
            Debug.Log("The pathTree is not empty on a new path set.");
        //pathCompletedTree.Push(pathCompletedCallback);
        updateCurrentStates += WalkPath;
    }

    public void UpdateDestination(Vector3 destination)
    {
        path = new Queue<Vector3>(PathFinder.FindPath(Position, destination));
    }

    void SetRandomDestination()
    {
        System.Random r = new System.Random();
        int x = r.Next(0, 12);
        int z = r.Next(0, 6);
        SetDestination(new Vector3(x, 0, z), On_NPC_PathCompleted);
    }

    void On_NPC_PathCompleted(NPC npc)
    {
        SetRandomDestination();
    }


    #region PathWalking

    CallbackTree<Action> pathCompletedTree;
    Action consumeNextNode;
    Action updatePathProgress;

    void WalkPath()
    {
        if (HasArrived)
        {
            if (path.Count > 0)
            {
                consumeNextNode();
            }
            else
                pathCompletedTree.Last()();
        }
        else
            UpdatePosAndRot();
    }

    void PathCompleted()
    {
        animator.Idle();
        updateCurrentStates -= WalkPath;
        pathCompleted(this);
        //pathCompleted = null;
    }
    float timer = 0;

    void ConsumeNextNode()
    {
        GridNode currentNode = GridController.GetNodeAt(Position);
        GridNode nextNode = GridController.GetNodeAt(path.Peek());
        IPathUnit otherUnit = nextNode.PathUnit;
        if (otherUnit != null)
        {
            if (otherUnit.OnPath)
            {
                if (otherUnit.GetDestinationNode() == currentNode)
                {
                    if (ResolveTraficConflictWith(otherUnit))
                    {
                        otherUnit.StepAwayFrom(nextNode, OnStepAwayCompleted);
                        // not working if the nextNode is the last in the path
                        updateCurrentStates -= WalkPath;
                    }
                    else
                    {
                        StepAwayFrom(currentNode, OnStepAwayCompleted);
                    }
                    return;
                }
                animator.Idle();
                updateCurrentStates -= WalkPath;
                updateCurrentStates += WaitingForNextNode;
                lookRotation = Quaternion.LookRotation(path.Peek() - Position, Vector3.up);
                return;
            }
            else
            {
                // FIXME: XD this will make no difference on the next Update
                path = new Queue<Vector3>(PathFinder.FindPath(Position, path.ToArray()[path.Count - 1]));
                if (path == null) Debug.LogError("the other dude is blocking the only way to the target");
            }
        }
        currentNode.SetPathUnit(null);
        nextNode.SetPathUnit(this);
        destination = path.Dequeue();
        Vector3 dir = destination - Position;
        lookRotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    void WaitingForNextNode()
    {
        timer += Time.deltaTime;
        if (timer > 0.35f)
        {
            timer = 0;
            GridNode nextNode = GridController.GetNodeAt(path.Peek());
            IPathUnit otherUnit = nextNode.PathUnit;
            if (otherUnit != null)
            {
                return;
            }
            GridController.GetNodeAt(Position).SetPathUnit(null);
            nextNode.SetPathUnit(this);
            animator.Walk();
            updateCurrentStates += WalkPath;
            updateCurrentStates -= WaitingForNextNode;
            destination = path.Dequeue();
            Vector3 dir = destination - Position;
        }
    }

    bool ResolveTraficConflictWith(IPathUnit other)
    {
        //if (UnityEngine.Random.value < 0.5f)
        //    return false;
        return true;
    }

    public void StepAwayFrom(GridNode from, Action<Action> stepAwayCompleted)
    {
        // TODO: make it to some sort of floodfill
        GridNode standOn = GridController.GetNodeAt(Position);
        foreach (GridNode neighbour in from.Neighbours)
        {
            if (neighbour.PathUnit == null && neighbour.Walkable && standOn != neighbour)
            {
                destination = neighbour.Position;
                lookRotation = Quaternion.LookRotation(destination - Position, Vector3.up);
                from.SetPathUnit(null);
                neighbour.SetPathUnit(this);
                pathCompletedTree.Stack(delegate
                {
                    StepAwayCompleted();
                    stepAwayCompleted(OnPassedBy);
                });
                updateCurrentStates -= WalkPath;
                updateCurrentStates += UpdateStepAway;
                return;
            }
        }
        Debug.LogError("Ay no neighbour is free" + from.Position);
    }

    void UpdateStepAway()
    {
        UpdatePosAndRot();

        if (HasArrived)
        {
            pathCompletedTree.Take()();
        }
    }

    void StepAwayCompleted()
    {
        animator.Idle();
        destination = path.Peek();
        lookRotation = Quaternion.LookRotation(destination - Position, Vector3.up);
        updateCurrentStates -= UpdateStepAway;
        updateCurrentStates += UpdateRotation;
    }

    void OnStepAwayCompleted(Action passedByCallback)
    {
        Debug.Log("stepaway completed");
        passedBy = passedByCallback;
        if (path.Count <= 2)
        {
            passNumber = 0;
            pathCompletedTree.Add(CheckPassability);
        }
        else consumeNextNode += CheckPassability;
        updateCurrentStates += WalkPath;
    }

    Action passedBy;
    void CheckPassability()
    {
        if (passNumber-- < 1)
        {
            Debug.Log("check pass has arrived");
            passNumber = 2;
            consumeNextNode -= CheckPassability;
            pathCompletedTree.Remove(CheckPassability);
            passedBy();
            passedBy = null;
        }
    }

    // The number of node required to let the other unit continue its path;
    int passNumber = 2;
    void OnPassedBy()
    {
        animator.Walk();
        // FIXME: it is possible that form the stepaway pos to the next path pos need more than 1 node to take
        //        but we can only handle 1 difference between the two.
        GridController.GetNodeAt(Position).SetPathUnit(null);
        GridController.GetNodeAt(path.Dequeue()).SetPathUnit(this);
        updateCurrentStates -= UpdateRotation;
        updateCurrentStates = WalkPath;
    }

    #endregion

    void UpdatePosAndRot()
    {
        Vector3 dir = destination - Position;
        Vector3 motion = dir.normalized * Time.deltaTime * moveSpeed;
        controller.Move(motion);
        UpdateRotation();
    }

    void UpdateRotation()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed);
    }


    public void LookFurniture()
    {
        StartCoroutine(nameof(LookAtFurniture));
    }

    IEnumerator LookAtFurniture()
    {
        float timer = 0;
        Quaternion lookDir = Quaternion.LookRotation(targetFurn.transform.position - Position);
        while (timer < 1)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, turnSpeed);
            yield return null;
        }
    }

    public IEnumerator SimulateWaiting(Action afterWaiting)
    {
        //yield return new WaitForSeconds(Random.Range(1f, 3f));
        yield return null;
        afterWaiting();
    }


    public void Destroy()
    {
        Vector3 at = path.Count > 0 ? destination : Position;
        GridController.GetNodeAt(at).SetPathUnit(null);

        Destroy(gameObject);
    }


    public GridNode GetDestinationNode()
    {
        return GridController.GetNodeAt(path.Peek());
    }
}