using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class PaymentPanelController : MonoBehaviour {

    public GameObject paymentPanel;
    public Button buyGoldButton;
    public GameObject buyGoldPage;
    public GameObject buyDiamondPage;
    public GameObject processPanel;

	void OnEnable()
    {
        buyGoldButton.Select();
        ClickBuyGoldButton();
        processPanel.SetActive(false);
    }


    public void ClickBuyGoldButton()
    {
        buyGoldPage.SetActive(true);
        buyDiamondPage.SetActive(false);
    }
    public void ClickBuyDiamondButton()
    {
        buyGoldPage.SetActive(false);
        buyDiamondPage.SetActive(true);
    }
    
    public void ClickExitButton()
    {
        paymentPanel.SetActive(false);
    }
    public void ClickBackButton()
    {
        processPanel.SetActive(false);
    }
}
