using Cook_lib;

public partial class ResultSDS : CsvBase, IResultSDS
{
    public float exceedTime;
    public bool isUniversal;
    public int maxNum;
    public int money;
    public int moneyOptimized;

    public int GetID()
    {
        return ID;
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
