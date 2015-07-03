public class CharacterViewModel : CharacterItemViewModel
{
	protected override void OnStateLoad()
	{
		_modelID = GameStorage.UsedCharacterID;
		base.OnStateLoad();
	}
}