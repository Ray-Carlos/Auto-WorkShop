using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class SliderControl : MonoBehaviour
{
    public Slider slider;
    public InputField inputField;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SliderChanged()
    {
        inputField.text = slider.value.ToString();
        text.text = (100 - slider.value).ToString();
    }

    public void InputFieldChanged()
    {
                if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.text = "0";
            return;
        }

        int.TryParse(inputField.text, out int value);

        if (value == 0)
        {
            inputField.text = "0";
        }
        else if (value > 100)
        {
            inputField.text = "100";
            value = 100;
        }
        else
        {
            inputField.text = value.ToString();
        }

        text.text = (100 - slider.value).ToString();
        slider.value = value;
        return;

    }
}
