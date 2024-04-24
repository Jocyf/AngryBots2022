using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public partial class TextAdventureManager : MonoBehaviour
{
    public Transform player;
    public MoodBox[] playableMoodBoxes;
    public float timePerChar;
    private int currentMoodBox;
    private int textAnimation;
    private float timer;
    private Vector3 camOffset;


    public virtual void Start()
    {
        if (!this.player)
        {
            this.player = GameObject.FindWithTag("Player").transform;
        }
        GameObject leftIcon = new GameObject("Left Arrow", new System.Type[] {typeof(Text)});
        GameObject rightIcon = new GameObject("Right Arrow", new System.Type[] {typeof(Text)});
        leftIcon.GetComponent<Text>().text = "< backspace";
        leftIcon.GetComponent<Text>().font = this.GetComponent<Text>().font;
        leftIcon.GetComponent<Text>().material = this.GetComponent<Text>().material;
        //leftIcon.GetComponent<Text>().anchor = TextAnchor.UpperLeft;
        leftIcon.gameObject.layer = LayerMask.NameToLayer("Adventure");

        {
            float _65 = 0.01f;
            Vector3 _66 = leftIcon.transform.position;
            _66.x = _65;
            leftIcon.transform.position = _66;
        }

        {
            float _67 = 0.1f;
            Vector3 _68 = leftIcon.transform.position;
            _68.y = _67;
            leftIcon.transform.position = _68;
        }
        rightIcon.GetComponent<Text>().text = "space >";
        rightIcon.GetComponent<Text>().font = this.GetComponent<Text>().font;
        rightIcon.GetComponent<Text>().material = this.GetComponent<Text>().material;
        //rightIcon.GetComponent<Text>().anchor = TextAnchor.UpperRight;
        rightIcon.gameObject.layer = LayerMask.NameToLayer("Adventure");

        {
            float _69 = 0.99f;
            Vector3 _70 = rightIcon.transform.position;
            _70.x = _69;
            rightIcon.transform.position = _70;
        }

        {
            float _71 = 0.1f;
            Vector3 _72 = rightIcon.transform.position;
            _72.y = _71;
            rightIcon.transform.position = _72;
        }
    }

    public virtual void OnEnable()
    {
        this.textAnimation = 0;
        this.timer = this.timePerChar;
        this.camOffset = Camera.main.transform.position - this.player.position;
        this.BeamToBox(this.currentMoodBox);
        if (this.player)
        {
            PlayerMoveController ctrler = this.player.GetComponent<PlayerMoveController>();
            ctrler.enabled = false;
        }
        this.GetComponent<Text>().enabled = true;
    }

    public virtual void OnDisable()
    {
        // and back to normal player control
        if (this.player)
        {
            PlayerMoveController ctrler = this.player.GetComponent<PlayerMoveController>();
            ctrler.enabled = true;
        }
        this.GetComponent<Text>().enabled = false;
    }

    public virtual void Update()
    {
        this.GetComponent<Text>().text = "AngryBots \n \n";
        this.GetComponent<Text>().text = this.GetComponent<Text>().text + this.playableMoodBoxes[this.currentMoodBox].data.adventureString.Substring(0, this.textAnimation);
        Debug.Log(this.GetComponent<Text>().text);
        if (this.textAnimation >= this.playableMoodBoxes[this.currentMoodBox].data.adventureString.Length)
        {
        }
        else
        {
            this.timer = this.timer - Time.deltaTime;
            if (this.timer <= 0f)
            {
                this.textAnimation++;
                this.timer = this.timePerChar;
            }
        }
        this.CheckInput();
    }

    public virtual void BeamToBox(int index)
    {
        if (index > this.playableMoodBoxes.Length)
        {
            return;
        }
        this.player.position = this.playableMoodBoxes[index].transform.position;
        Camera.main.transform.position = this.player.position + this.camOffset;
        this.textAnimation = 0;
        this.timer = this.timePerChar;
    }

    public virtual void CheckInput()
    {
        int input = 0;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            input = 1;
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                input = -1;
            }
        }
        if (input != 0)
        {
            if (this.textAnimation < this.playableMoodBoxes[this.currentMoodBox].data.adventureString.Length)
            {
                this.textAnimation = this.playableMoodBoxes[this.currentMoodBox].data.adventureString.Length;
                input = 0;
            }
        }
        if (input != 0)
        {
            if (((this.currentMoodBox - this.playableMoodBoxes.Length) == -1) && (input < 0))
            {
                input = 0;
            }
            if ((this.currentMoodBox == 0) && (input < 0))
            {
                input = 0;
            }
            if (input != 0)
            {
                this.currentMoodBox = (input + this.currentMoodBox) % this.playableMoodBoxes.Length;
                this.BeamToBox(this.currentMoodBox);
            }
        }
    }

    public TextAdventureManager()
    {
        this.timePerChar = 0.125f;
        this.camOffset = Vector3.zero;
    }

}