using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugInfos : MonoBehaviour
{
    [SerializeField]
    private Spaceship spaceship;

    [SerializeField]
    private Text _label, _infoLine1, _infoLine2, _infoLine3;

    public Spaceship Spaceship { get => spaceship; set => spaceship = value; }
    public Text Label { get => _label; set => _label = value; }
    public Text InfoLine1 { get => _infoLine1; set => _infoLine1 = value; }
    public Text InfoLine2 { get => _infoLine2; set => _infoLine2 = value; }
    public Text InfoLine3 { get => _infoLine3; set => _infoLine3 = value; }

    // Start is called before the first frame update
    void Start()
    {
        Spaceship = GameObject.FindObjectOfType<Spaceship>();

        Label = transform.GetChild(0).GetComponent<Text>();
        InfoLine1 = transform.GetChild(1).GetComponent<Text>();
        InfoLine2 = transform.GetChild(2).GetComponent<Text>();
        InfoLine3 = transform.GetChild(3).GetComponent<Text>();

        Label.text = $"PitchYaw values";
        InfoLine3.text = "";
    
    }

    // Update is called once per frame
    void Update()
    {
        InfoLine1.text = $"X: {Spaceship.PitchYaw.x}";
        InfoLine2.text = $"Y: {Spaceship.PitchYaw.y}";
    }
}
