public class LandViewModel : LandItemViewModel 
{
	protected override void OnStateLoad()
	{
		_modelID = GameStorage.UsedLandID;
		base.OnStateLoad();
	}
}
