using UnityEngine;
public class Inventory : MonoBehaviour
{

    public GameObject inventory;
    public GameObject itemManager;
    public GameObject slotHolder;

    private int slots;
    private Transform[] slot;
    private GameObject itemPicker;
    private bool inventoryEnabled;
    private bool itemAdded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slots = slotHolder.transform.childCount;
        slot = new Transform[slots];
        DetectInventorySlots();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryEnabled = !inventoryEnabled;
        }

        if (inventoryEnabled)
        {
            inventory.GetComponent<Canvas>().enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            inventory.GetComponent<Canvas>().enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            print(other.name);
            itemPicker = other.gameObject;
            AddItem(itemPicker);
        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Item"))
        {
            itemAdded = false;
        }
    }

    public void AddItem(GameObject item)
    {
        print(item.name);
        for (int i = 0; i < slots; i++) {
            if (slot[i].GetComponent<Slot>().empty && itemAdded==false)
            {
                slot[i].GetComponent<Slot>().item = itemPicker;
                slot[i].GetComponent<Slot>().itemIcon = itemPicker.GetComponent<Item>().icon;
                item.transform.parent = itemManager.transform;
                item.transform.position= itemManager.transform.position;
                if (item.GetComponent<MeshRenderer>())
                {
                    item.GetComponent<MeshRenderer>().enabled = false;
                }
                Destroy(item.GetComponent<Rigidbody>());

                itemAdded = true;
            }
        }
    }

    public void DetectInventorySlots()
    {
        inventory.GetComponent<Canvas>().enabled = true;

        for (int i = 0; i < slots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i);
            print(slot);
        }
    }
}
