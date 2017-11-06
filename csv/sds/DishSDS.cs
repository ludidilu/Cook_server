using Cook_lib;

public class DishSDS : CsvBase, IDishSDS
{
    public float prepareTime;
    public float prepareDecreaseValue;
    public float cookTime;
    public float optimizedTime;
    public float optimizeDecreaseValue;
    public float exceedTime;
    public bool isUniversal;
    public int maxNum;
    public int money;
    public int moneyOptimized;

    public int GetID()
    {
        return ID;
    }

    public float GetPrepareTime()
    {
        return prepareTime;
    }

    public float GetPrepareDecreaseValue()
    {
        return prepareDecreaseValue;
    }

    public float GetCookTime()
    {
        return cookTime;
    }

    public float GetOptimizeTime()
    {
        return optimizedTime;
    }

    public float GetOptimizeDecreaseValue()
    {
        return optimizeDecreaseValue;
    }

    public float GetExceedTime()
    {
        return exceedTime;
    }

    public bool GetIsUniversal()
    {
        return isUniversal;
    }
    public int GetMaxNum()
    {
        return maxNum;
    }

    public int GetMoney()
    {
        return money;
    }

    public int GetMoneyOptimized()
    {
        return moneyOptimized;
    }
}
