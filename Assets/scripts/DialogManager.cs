using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[RequireComponent(typeof(DialogLoadXml))]
public class DialogManager : MonoBehaviour
{

    string currentXml;
    string currentEvent;

    Dictionary<string, Dialog> dialogs;
    CanvasGroup dlgCanvas;
    public CanvasGroup dlgHelpCanvas;
    public Text dlgMessage;
    private int currentIndex = 0;
    public float timerNextMessage = 2.0f;
    private float timerCurrentMessage = 0.0f;

    private bool nextMessage = false;
    private bool stopEvent = false;
    private bool checkForNext = false;

    private float durationTimer = 0.0f;

    public GameObject chatBoxIcn1;
    public GameObject chatBoxIcn2;
    public GameObject particle;

    public AudioSource audioSource;
    private bool hasPlayed = false;

    // Use this for initialization
    void Start()
    {
        dlgCanvas = GetComponent<CanvasGroup>();
        currentXml = Application.dataPath + "/dialog_system.xml";
        currentEvent = "";

        DialogLoadXml loadXml = new DialogLoadXml(currentXml);
        dialogs = loadXml.parseXml();
        if (dlgMessage)
            dlgMessage.text = currentXml;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogs != null && dialogs.ContainsKey(currentEvent))
        {
            Dialog dlg = dialogs[currentEvent];

            if (currentIndex + 1 > dlg.persons.Count)
            {
                stopEvent = false;
                nextMessage = false;
                return;
            }
            //we have event in dialogs
            if (!nextMessage && !stopEvent && dlgCanvas.alpha < 1.0f)
            {
                float fade_in = dlg.persons[currentIndex].fade_in;

                if (dlg.persons[currentIndex].help == 0)
                {
                    dlgCanvas.alpha += (1.0f * Time.deltaTime) / fade_in;
                    Color c1 = chatBoxIcn1.GetComponent<SpriteRenderer>().color;
                    Color c2 = chatBoxIcn2.GetComponent<SpriteRenderer>().color;
                    c1.a += (1.0f * Time.deltaTime) / (fade_in / 2);
                    c2.a += (1.0f * Time.deltaTime) / fade_in;

                    if (c1.a > 0.5f)
                    {
                        float startsize = particle.GetComponent<ParticleSystem>().startSize;
                        startsize += (1.0f * Time.deltaTime) / (fade_in / 2);
                        if (startsize <= 1.0f)
                            particle.GetComponent<ParticleSystem>().startSize = startsize;
                    }
                    if (dlgCanvas.alpha > 1.0f)
                        dlgCanvas.alpha = 1.0f;
                    if (c1.a > 1.0f)
                        c1.a = 1.0f;
                    if (c2.a > 1.0f)
                        c2.a = 1.0f;

                    chatBoxIcn1.GetComponent<SpriteRenderer>().color = c1;
                    chatBoxIcn2.GetComponent<SpriteRenderer>().color = c2;

                    string text = dlg.persons[currentIndex].text;
                    dlgMessage.text = text;
                }
                else
                {
                    dlgHelpCanvas.alpha += (1.0f * Time.deltaTime) / fade_in;
                    if (dlgHelpCanvas.alpha > 1.0f)
                        dlgHelpCanvas.alpha = 1.0f;
                }

            }
            else if (stopEvent || nextMessage)
            {
                float alpha = 0.0f;
                float fade_out = dlg.persons[currentIndex].fade_out;

                //Todo: kinda shitty solution... do it smarter
                if (dlg.persons[currentIndex].help == 0)
                {
                    dlgCanvas.alpha -= (1.0f * Time.deltaTime) / fade_out;
                    Color c1 = chatBoxIcn1.GetComponent<SpriteRenderer>().color;
                    Color c2 = chatBoxIcn2.GetComponent<SpriteRenderer>().color;
                    c1.a -= (1.0f * Time.deltaTime) / fade_out;
                    c2.a -= (1.0f * Time.deltaTime) / fade_out;

                    float startsize = particle.GetComponent<ParticleSystem>().startSize;
                    startsize -= (1.0f * Time.deltaTime) / (fade_out / 2);
                    if (startsize >= 0.0f)
                        particle.GetComponent<ParticleSystem>().startSize = startsize;

                    if (dlgCanvas.alpha < 0.0f)
                        dlgCanvas.alpha = 0.0f;
                    if (c1.a < 0.0f)
                        c1.a = 0.0f;
                    if (c2.a < 0.0f)
                        c2.a = 0.0f;

                    chatBoxIcn1.GetComponent<SpriteRenderer>().color = c1;
                    chatBoxIcn2.GetComponent<SpriteRenderer>().color = c2;

                    alpha = dlgCanvas.alpha;
                }
                else
                {
                    dlgHelpCanvas.alpha -= (1.0f * Time.deltaTime) / fade_out;
                    if (dlgHelpCanvas.alpha < 0.0f)
                        dlgHelpCanvas.alpha = 0.0f;

                    alpha = dlgHelpCanvas.alpha;
                }

                if (alpha <= 0.01f)
                {
                    if (nextMessage)
                        currentIndex++;
                    else
                        currentEvent = "";
                    nextMessage = false;
                    if (stopEvent)
                        currentIndex = dlg.persons.Count + 1;
                    stopEvent = false;
                    hasPlayed = false;
                }

            }
            else if (dlgCanvas.alpha >= 1.0f)
            {
                if (checkForNext)
                {
                    //after duration check for next
                    timerCurrentMessage += Time.deltaTime;
                    if (timerNextMessage <= timerCurrentMessage)
                    {
                        nextMessage = true;
                        timerCurrentMessage = 0.0f;
                        durationTimer = 0.0f;
                        checkForNext = false;
                    }
                }
                else
                {
                    //stay alpha 1 as long as duration goes
                    float duration = dlg.persons[currentIndex].duration;
                    if (duration <= durationTimer)
                    {
                        checkForNext = true;
                    }
                    durationTimer += Time.deltaTime;
                }
            }

        }
    }

    void msg_eventTrigger(string nextEvent)
    {
        currentEvent = nextEvent;
        stopEvent = false;
        currentIndex = 0;
        checkForNext = false;
        durationTimer = 0.0f;

        if (!hasPlayed)
        {
            audioSource.PlayOneShot((AudioClip)Resources.Load("Sounds/" + nextEvent));
            hasPlayed = true;
        }


    }
    void msg_eventTriggerEnd(string nextEvent)
    {
        if (currentEvent != "" && nextEvent == currentEvent)
        {
            stopEvent = true;
        }
    }
}
