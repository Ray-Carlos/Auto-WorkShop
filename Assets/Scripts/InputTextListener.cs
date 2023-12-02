using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class InputTextListener : MonoBehaviour
{
    public InputField inputField;

    int max = 10;

    public void RestrictedRange()
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
        else if (value > max)
        {
            inputField.text = "10";
        }
        else
        {
            inputField.text = value.ToString();
        }
        return;
    }
}
