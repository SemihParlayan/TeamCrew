using UnityEngine;
using System.Collections;
using System.Linq;

public class SelectedFrog : MonoBehaviour 
{
    public int startIndex;
    public CharacterSelectionScreen selection;
    [HideInInspector]
    public Transform selectedFrog;
    public Transform spawnPosition;

    public int index;
    private string player = "";

    void Awake()
    {
        player = GetComponent<M_Screen>().playerControl;
    }
    void Start()
    {
        index = startIndex;
    }

    public void OnLeft()
    {
        SelectFrog(-1);
    }
    public void OnRight()
    {
        SelectFrog(1);
    }

    public void OnJoin()
    {
        SelectFrog(0);
    }
    public void OnDeselect()
    {
        selectedFrog.gameObject.SetActive(false);
        selectedFrog = null;
    }
    private void SelectFrog(int indexDirection)
    {
        Transform newFrog = selection.GetFrog(ref index, indexDirection);
        if (newFrog == null)
            return;

        newFrog.position = spawnPosition.position;
        newFrog.GetComponentInChildren<FrogPrototype>().player = player;
        selectedFrog = newFrog;
    }
}
