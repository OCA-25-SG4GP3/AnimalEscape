using UnityEngine;

public class ColorPanelPuzzle : MonoBehaviour
{
    ColorPanelManager colorPanelManager;
    [SerializeField] private ColorPanelPuzzle pairPanel;
    Animator animator;

    public bool isStepped = false; //“¥‚Ü‚¦‚½‚©‚Ç‚¤‚©

    void Awake()
    {
        colorPanelManager = FindAnyObjectByType<ColorPanelManager>();
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO need cache to reduce lag ?
            var playerInfo = other.GetComponent<PlayerInfo>();
            if (!playerInfo.IsFallingDown()) return;
            print("pressed");
            isStepped = true;
            animator.Play("ColorPanelPressedAnim");
            if (pairPanel.isStepped)
            {
                colorPanelManager.AccumulatePoint();
                Destroy(pairPanel.gameObject);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isStepped) return;
            isStepped = false;
            animator.Play("ColorPanelReleasedAnim");
        }
    }



}
