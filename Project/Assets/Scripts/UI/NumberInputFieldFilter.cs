#region

using System;
using TMPro;
using UnityEngine;

#endregion

public class NumberInputFieldFilter : MonoBehaviour
{
    #region Events

    public event EventHandler OnInputFieldChanged;

    #endregion

    #region Variables & References

    [SerializeField] private string allowedSymbols = "0123456789";
    private TMP_InputField inputField;

    #endregion

    #region Initialization

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        inputField.onValueChanged.AddListener(newString =>
        {
            var filteredString = "";
            foreach (var newStringChar in newString)
            {
                if (!allowedSymbols.Contains(newStringChar)) continue;

                filteredString += newStringChar;
            }

            inputField.SetTextWithoutNotify(filteredString);

            OnInputFieldChanged?.Invoke(this, EventArgs.Empty);
        });
    }

    #endregion

    #region Set

    public void SetIntValue(int newIntValue)
    {
        if (newIntValue < 0)
            newIntValue = 0;

        inputField.text = newIntValue.ToString();
    }

    #endregion

    #region Get

    public int GetInputFieldIntValue()
    {
        int.TryParse(inputField.text, out var inputFieldIntValue);
        return inputFieldIntValue;
    }

    #endregion
}