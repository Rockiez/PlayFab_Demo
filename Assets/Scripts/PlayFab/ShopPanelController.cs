using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
public class ShopPanelController : MonoBehaviour
{

    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject shopPanel;
    public GameObject shopLoadingWindow;
    public Text goldCurrencyCount;
    public Text diamondCurrencyCount;

    public Text UserMessageLabel;


    public GameObject[] shopItemsPanel;

    public GameObject previousButton;
    public GameObject nextButton;
    public Text pageMessage;

    public static List<CatalogItem> shopItems;
    public static List<ItemInstance> userItems;

    private int itemsLength;
    private const int itemsPerPage = 6;
    private int currentPageNumber;
    private int maxPageNumber;

    void OnEnable()
    {
		shopLoadingWindow.SetActive(true);
        foreach (GameObject go in shopItemsPanel)
        {
            go.SetActive(false);
        }

        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetUserInventory, OnPlayFabError);

    }

    void OnGetUserInventory(GetUserInventoryResult result)
    {
        userItems = result.Inventory;

        GetCatalogItemsRequest request = new GetCatalogItemsRequest()
        {
            CatalogVersion = PlayFabUserData.catalogVersion
        };
        PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogItems, OnPlayFabError);
    }

    void OnGetCatalogItems(GetCatalogItemsResult result)
    {
        List<CatalogItem> temp = result.Catalog;
        shopItems = temp;
        itemsLength = temp.Count;
        currentPageNumber = 1;
        maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowItems();
        shopLoadingWindow.SetActive(false);
    }

    void ButtonControl()
    {
        if (currentPageNumber == 1)
            previousButton.SetActive(false);
        else
            previousButton.SetActive(true);
        if (currentPageNumber == maxPageNumber)
            nextButton.SetActive(false);
        else
            nextButton.SetActive(true);
    }
    public void ShowItems()
    {
        int start, end, i, j;
        start = (currentPageNumber - 1) * itemsPerPage;
        if (currentPageNumber == maxPageNumber)
            end = itemsLength;
        else
            end = start + itemsPerPage;
        Text[] texts;
        Button button;
        for (i = start, j = 0; i < end; i++, j++)
        {
            int itemNum = i;
            texts = shopItemsPanel[j].GetComponentsInChildren<Text>();
            button = shopItemsPanel[j].GetComponentInChildren<Button>();
            texts[0].text = shopItems[i].DisplayName;

            if (shopItems[i].VirtualCurrencyPrices.ContainsKey("AU"))
            {
                texts[1].text ="Gold"+ shopItems[i].VirtualCurrencyPrices["AU"].ToString();
            }
            else if (shopItems[i].VirtualCurrencyPrices.ContainsKey("GM"))
            {
                texts[1].text ="Dia"+ shopItems[i].VirtualCurrencyPrices["GM"].ToString();
            }
            button.onClick.RemoveAllListeners();


            bool hasItems = false;
            foreach (ItemInstance ii in userItems)
            {
                if (ii.ItemClass == shopItems[i].ItemClass && !shopItems[i].IsStackable)
                {
                    hasItems = true;
                    break;
                }
            }
            if (hasItems)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "Owned";
            }
            else
            {
                button.interactable = true;
                button.GetComponentInChildren<Text>().text = "Buy";

                var item = shopItems[itemNum];
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate {
                    if (item.VirtualCurrencyPrices.ContainsKey("AU"))
                    {
                        PurchaseItemRequest request = new PurchaseItemRequest()
                        {
                            CatalogVersion = PlayFabUserData.catalogVersion,
                            VirtualCurrency = "AU",
                            Price = (int)item.VirtualCurrencyPrices["AU"],
                            ItemId = item.ItemId
                        };
                        PlayFabClientAPI.PurchaseItem(request, OnPurchaseItem, OnPlayFabPurchaseError);
                        UserMessageLabel.text = "Purchasing...";
                    }
                    else if (item.VirtualCurrencyPrices.ContainsKey("GM"))
                    {
                        PurchaseItemRequest request = new PurchaseItemRequest()
                        {
                            CatalogVersion = PlayFabUserData.catalogVersion,
                            VirtualCurrency = "GM",
                            Price = (int)item.VirtualCurrencyPrices["GM"],
                            ItemId = item.ItemId
                        };
                        PlayFabClientAPI.PurchaseItem(request, OnPurchaseItem, OnPlayFabPurchaseError);
                        UserMessageLabel.text = "Purchasing...";
                    }
                });
            }
            shopItemsPanel[j].SetActive(true);
        }
        for (; j < itemsPerPage; j++)
            shopItemsPanel[j].SetActive(false);
    }


    void OnPurchaseItem(PurchaseItemResult result)
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetUserInventoryPurchaseItem, OnPlayFabError);
    }

    void OnGetUserInventoryPurchaseItem(GetUserInventoryResult result)
    {
        goldCurrencyCount.text = result.VirtualCurrency["AU"].ToString();
        diamondCurrencyCount.text = result.VirtualCurrency["GM"].ToString();
        UserMessageLabel.text = "Purchase Success";

        userItems = result.Inventory;
        shopPanel.GetComponent<ShopPanelController>().ShowItems();
    }
    
    void OnPlayFabPurchaseError(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
        UserMessageLabel.text = "Purchase Fail";
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
    }

    public void ClickPreviousButton()
    {
        currentPageNumber--;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowItems();
    }

    public void ClickNextButton()
    {
        currentPageNumber++;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowItems();
    }
}
