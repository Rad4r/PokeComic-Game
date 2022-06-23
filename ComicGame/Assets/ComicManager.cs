using UnityEngine;
using UnityEngine.UI;

public class ComicManager : MonoBehaviour
{
    [SerializeField] private Transform panelParent;
    [SerializeField] private float moveSpeed;
    // private Image[] panels;
    private PanelBoard[] panels;
    [SerializeField] private int currentPanelNumber;
    [SerializeField] private GameObject startCanvas;

    private GameManager GM;

    void Start()
    {
        GM = FindObjectOfType<GameManager>();
        panels = panelParent.GetComponentsInChildren<PanelBoard>();
        // DisplayPanel();
    }

    void Update()
    {
        // Mathf.Clamp(currentPanelNumber, 0, panels.Length);

        // Debug.Log("Distance:" + Vector3.Distance(Camera.main.transform.position, panels[currentPanelNumber].transform.position + new Vector3(0, 0, -10)));

        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        
        if(currentPanelNumber >= 1)
            startCanvas.SetActive(false);
        
        if (!GM.battleDone)
            ChangePanel(); //Add a delay between each

        if (Vector3.Distance(Camera.main.transform.position,
            panels[currentPanelNumber].transform.position + new Vector3(0, 0, -10)) > 0)
            return;
        
        if (currentPanelNumber < panels.Length - 1 && currentPanelNumber != 6 && Input.GetButtonDown("GreenButton")) //Input.GetButtonDown("Submit") Disable during the battle and choice panel
            ChangePanelNumber(currentPanelNumber + 1);
        
        // if (currentPanelNumber > 0 && Input.GetButtonDown("Cancel"))
        //     ChangePanelNumber(currentPanelNumber - 1);
    }

    public void ChangePanelNumber(int numb)
    {
        currentPanelNumber = numb;
    }

    void ChangePanel() //messing with the other change
    {
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position,
                            panels[currentPanelNumber].transform.position + new Vector3(0, 0, -10), moveSpeed * Time.deltaTime );
        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, panels[currentPanelNumber].GetComponent<SpriteRenderer>().bounds.size.y / 1.7f, Time.deltaTime);
    }

    public int GetCurrentPanelNumber()
    {
        return currentPanelNumber;
    }

    public PanelBoard GetCurrentPanelBoard()
    {
        return panels[currentPanelNumber];
    }
    
    // void DisplayPanel() //Move to
    // {
    //     Debug.Log(currentPanelNumber);
    //     
    //     // if (currentPanelNumber < 0 || currentPanelNumber >= panels.Length)
    //     // {
    //     //     return;
    //     // }
    //     
    //     // Debug.Log( "Transform size is: " + panels[currentPanelNumber].GetComponent<SpriteRenderer>().bounds.size);
    //     // Debug.Log( "Rect size pixels is: " + panels[currentPanelNumber].GetComponent<SpriteRenderer>().sprite.rect.size);
    //     // Debug.Log( "Pixel per Unit is: " + panels[currentPanelNumber].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
    //
    //     // Vector3.MoveTowards(Camera.main.transform.position,
    //     //     panels[currentPanelNumber].transform.position + new Vector3(0, 0, -10), .5f);
    //     
    //     // Camera.main.transform.position = panels[currentPanelNumber].transform.position + new Vector3(0, 0, -10);
    //     // Camera.main.orthographicSize = panels[currentPanelNumber].GetComponent<SpriteRenderer>().bounds.size.y / 1.7f;
    //     // Debug.Log( "Camera size: " + Camera.main.orthographicSize);
    //     
    //     
    //     // 256x300 is actual size
    //     
    //     // Use rect siz with transform scale
    //     
    //     // Also pixel per unit (256 pixels per unit)
    //     // 256
    //     // transform scale of the object transform
    // }
    
}
