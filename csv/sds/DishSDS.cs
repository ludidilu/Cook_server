using Cook_lib;

public partial class DishSDS : CsvBase, IDishSDS
{
    public float prepareTime;
    public float prepareDecreaseValue;
    public float cookTime;
    public float optimizeTime;
    public float optimizeDecreaseValue;
    public int resultID;

    private ResultSDS resultSDS;

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
        return optimizeTime;
    }

    public float GetOptimizeDecreaseValue()
    {
        return optimizeDecreaseValue;
    }

    public IResultSDS GetResult()
    {
        if (resultSDS == null)
        {
            resultSDS = StaticData.GetData<ResultSDS>(resultID);
        }

        return resultSDS;
    }
}
