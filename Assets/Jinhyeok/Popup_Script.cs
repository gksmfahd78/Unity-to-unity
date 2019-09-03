using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Popup_Script : MonoBehaviour {

    public static Popup_Script instance;
    public GameObject MyObject;
    [SerializeField] private Text _title;
    [SerializeField] private Text _description;
    [SerializeField] private Button _button;
    private Text _buttonText;

    private void Awake()
    {
        if (Popup_Script.instance == null)
            Popup_Script.instance = this;

        _buttonText = _button.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        MyObject.SetActive(false);
    }

    public void Show(string title, string description, string buttonCaption, Action buttonCallback)
    {
        _title.text = title;
        _description.text = description;

        _button.gameObject.SetActive(buttonCallback != null);

        if (buttonCallback != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => buttonCallback());
            _buttonText.text = buttonCaption;
        }

        MyObject.SetActive(true);
    }
}
