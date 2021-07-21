using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    static NPCController instance;

    [SerializeField]
    NPC npcPrefab;
    [SerializeField]
    LayerMask layerMask;

    MouseController mouseC;

    public static NPCController Instance => instance;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mouseC = MouseController.Instace;
    }
    // how to create awaitable objects
    // pointer, adresses, fixed

    public NPC Spawn(Vector3 position)
    {
        NPC npc = Instantiate(npcPrefab, position, Quaternion.identity);
        return npc;
    }


    void NPC_ArrivedToFurniture(NPC npc)
    {
        npc.ChooseItem();
    }

   public void NPC_ReadyToPurchase(NPC costumer)
    {
        costumer.WaitForPurchase();
    }

    public void NPC_LeftTheShop(NPC costumer)
    {
        costumer.Destroy();
    }

    void Update()
    {
        if (mouseC.IsPointerOver)
        {
            return;
        }

        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit info, 1000f, layerMask))
            {
                NPC npc = Spawn(info.point);
                Furniture target = Furniture.Furnitures[new System.Random().Next(0, Furniture.Furnitures.Count)];
                npc.TargetFurniture = target;
                npc.SetDestination(target.GetRandomVisitableGridNode().Position,
                                   (test) => { NPC_ArrivedToFurniture(test); });
            }
        }
    }
}