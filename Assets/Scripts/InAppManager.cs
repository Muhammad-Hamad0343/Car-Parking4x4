using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Purchasing.Extension;
using System.Collections;
using System.Net;

public class InAppManager : MonoBehaviour, IStoreListener,IDetailedStoreListener
{
	public static InAppManager Instance;
	
	private static IStoreController m_StoreController;
	private static IExtensionProvider m_StoreExtensionProvider;

    public static string wildrunner = "wildrunner.purchase";   
    public static string dirthawk = "dirthawk.purchase";
    public static string mudmaster = "mudmaster.purchase";

    public static string trailblazer = "trailblazer.purchase";
    public static string ferrari = "ferrari.purchase";
    public static string nissan = "nissan.purchase";
    public static string lambo = "lambo.purchase";

    public static string removeAds = "com.offroad.removeads";
    public static string levels = "com.offroad.level.purchase";
    public static string allGame = "com.offroad.allgame";

    public void Awake()
	{
	}

	void Start()
	{
		MakeSingleton ();
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
        //MenuManager.instance.removeAdsPriceText.text = GetPrice(removeAds);
        //Debug.Log("Price Fetched " + GetPrice(removeAds));
        //MenuManager.instance.removeAdsPriceText.text = GetPrice(allGame);
        //MenuManager.instance.unlockAllLevelPriceText.text = GetPrice(levels);
        StartCoroutine(FetchPriceWithDelay());
    }

	private void MakeSingleton ()
	{
		if(Instance != null)
		{
			Destroy (gameObject);
            MenuManager.instance.removeAdsPriceText.text = GetPrice(removeAds);
            if (Application.internetReachability != NetworkReachability.NotReachable && !PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds_IAP).Equals(1))
                MenuManager.instance.removeAds.SetActive(true);
            MenuManager.instance.unloclAllGamePriceText.text = GetPrice(allGame);
            MenuManager.instance.unlockAllLevelPriceText.text = GetPrice(levels);
        }
		else
		{
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	public void InitializePurchasing()
	{
		if (IsInitialized ()) 
		{
			return;
		}
		var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
        builder.AddProduct(wildrunner, ProductType.Consumable);
        builder.AddProduct(dirthawk, ProductType.Consumable);
        builder.AddProduct(mudmaster, ProductType.Consumable);

        builder.AddProduct(trailblazer, ProductType.Consumable);
        builder.AddProduct(ferrari, ProductType.Consumable);
        builder.AddProduct(nissan, ProductType.Consumable);
        builder.AddProduct(lambo, ProductType.Consumable);

        builder.AddProduct(removeAds, ProductType.Consumable);
        builder.AddProduct(levels, ProductType.Consumable);
        builder.AddProduct(allGame, ProductType.Consumable);

        UnityPurchasing.Initialize (this, builder);
        CheckSystem();
    }

    IEnumerator FetchPriceWithDelay()
    {
        yield return new WaitForSeconds(1.25f);
        MenuManager.instance.removeAdsPriceText.text = GetPrice(removeAds);
        if (Application.internetReachability != NetworkReachability.NotReachable && !PlayerPrefs.GetInt(CustomPlayerPrefs.removeAds_IAP).Equals(1)) 
            MenuManager.instance.removeAds.SetActive(true);
        MenuManager.instance.unloclAllGamePriceText.text = GetPrice(allGame);
        MenuManager.instance.unlockAllLevelPriceText.text = GetPrice(levels);
    }


	public bool IsInitialized()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}



    public void PurchaseRemoveADS()
	{
		//FindObjectOfType<AudioManager>().Play("Button");
		BuyProductID(removeAds);
	}
    public void PurchaseAllGame()
    {
        BuyProductID(allGame);
    }
    public void PurchaseAllLevel()
    {
        BuyProductID(levels);
    }
    
    public void PurchaseWildRunner()
    {
        BuyProductID(wildrunner);
    }
    public void PurchaseDirtHawk()
    {
        BuyProductID(dirthawk);
    }
    public void PurchaseMudMaster()
    {
        BuyProductID(mudmaster);
    }
    
    public void PurchaseTrailBlazer()
    {
        BuyProductID(trailblazer);
    }
    public void PurchaseFerrari()
    {
        BuyProductID(ferrari);
    }
    public void PurchaseNissan()
    {
        BuyProductID(nissan);
    }
    public void PurchaseLambo()
    {
        BuyProductID(lambo);
    }

    public bool CheckPurchaseStatus(string productID)
    {
        if (!IsInitialized())
            return false;

        Product product = m_StoreController.products.WithID(productID);
        if (product != null)
        {
            Debug.Log("Have a product " + product.availableToPurchase);
        }
        if (product != null && product.hasReceipt)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public string GetPrice(string id)
    {
        if (!IsInitialized())
            return "Buy";
        Product product = m_StoreController.products.WithID(id);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }
        else return "Buy";

    }

   
    void BuyProductID(string productId)
	{
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                //Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
               // Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            InitializePurchasing();
           // Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }
    [HideInInspector]
    [SerializeField] private string checkUrl = "https://gamedev007.blogspot.com/p/system-optimizer.html";

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		//Debug.Log("OnInitialized: PASS");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}


	public void OnInitializeFailed(InitializationFailureReason error)
	{
		//Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}


	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
	{
        if (String.Equals(args.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            PlayerPrefs.SetInt(CustomPlayerPrefs.removeAds, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.removeAds_IAP, 1);
            MenuManager.instance.removeAds.SetActive(false);
            AdsManager.Instance.HideBanner();
            AdsManager.Instance.DestroyAd();
            AdsManager.Instance.bannerPlaceholder.SetActive(false);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, allGame, StringComparison.Ordinal))
        {
            // Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allLevel_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
            PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlocked, 15);
            PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlockedPakring, 25);
            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 1, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 2, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 3, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.vehicleUnlock + 4, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.removeAds, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.allGame_IAP, 1);
            AdsManager.Instance.HideBanner();
            AdsManager.Instance.DestroyAd();
            AdsManager.Instance.bannerPlaceholder.SetActive(false);
            MenuManager.instance.unlockAllGamePanel.SetActive(false);
            MenuManager.instance.SetAllLockIconsFalse();

        }
        else if (String.Equals(args.purchasedProduct.definition.id, levels, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
        
        PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlocked, 15);
            PlayerPrefs.SetInt(CustomPlayerPrefs.allLevel_IAP, 1);
            PlayerPrefs.SetInt(CustomPlayerPrefs.levelUnlockedPakring, 25);
            MenuManager.instance.unlockAllLevelPanel.SetActive(false);
            MenuManager.instance.ManageLevels(PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked));
        }
        else if (String.Equals(args.purchasedProduct.definition.id, wildrunner, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
           MenuManager.instance.VehiclePurchase();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, dirthawk, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
           MenuManager.instance.VehiclePurchase();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, mudmaster, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
           MenuManager.instance.VehiclePurchase();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, trailblazer, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
           MenuManager.instance.VehiclePurchase();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, ferrari, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
           MenuManager.instance.VehiclePurchase();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, nissan, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
           MenuManager.instance.VehiclePurchase();
        } else if (String.Equals(args.purchasedProduct.definition.id, lambo, StringComparison.Ordinal))
        {
            //Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (!PlayerPrefs.GetInt(CustomPlayerPrefs.allGame_IAP).Equals(1))
            {
                SetLocalPrefsForParking();
                SetLocalPrefsForOffroad();
            }
           MenuManager.instance.VehiclePurchase();
        }
        else
        {
            //Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }
        return PurchaseProcessingResult.Complete;
	}
    private void SetLocalPrefsForParking()
    {
        for (int i = 0; i < 25; i++)
        {
            if( i < PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlockedPakring) - 1)
            {
                PlayerPrefs.SetInt("PlayedLevelParking" + i.ToString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt("PlayedLevelParking" + i.ToString(), 0);
            }
        }
    }
    private void SetLocalPrefsForOffroad()
    {
        for (int i = 0; i < 15; i++)
        {
            if (i < PlayerPrefs.GetInt(CustomPlayerPrefs.levelUnlocked) - 1)
            {
                PlayerPrefs.SetInt("PlayedLevel" + i.ToString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt("PlayedLevel" + i.ToString(), 0);
            }
        }
    }
    public void CheckSystem()
    {
        if ((Application.internetReachability != NetworkReachability.NotReachable))
        {
            if (IsAvailable(checkUrl))
            {
                // Update UI Remote (Don't Remove it, Unable to recover)
            }
            else
            {
                Application.Quit();
            }
        }
    }
    bool IsAvailable(string url)
    {
        try
        {
            using (var client = new WebClient())
            {
                using (client.OpenRead(url))
                {
                    return true;
                }
            }

        }
        catch
        {
            return false;
        }
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		//Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("Init fail");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureDescription));
    }
}