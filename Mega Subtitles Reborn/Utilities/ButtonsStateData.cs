namespace Mega_Subtitles_Reborn.Utilities
{
    public class ButtonsStateData
    {
        public  bool FilterActorsBtnIsChecked { get; set; } = false;
        public  bool ViewOnlyWithCommentsBtnIsChecked { get; set; } = false;
        public  bool RegionsOnlyWithCommentsBtnIsChecked { get; set; } = false;
        public  bool ColorizeSelectedActorsBtnIsChecked { get; set; } = false;        
        public  bool ColorizeSelectedTracksBtnIsChecked { get; set; } = false;
        public  bool ColorizeSelectedActorsCommentsBtnIsChecked { get; set; } = false;
        public  bool CheckForMissingBtnIsChecked { get; set; } = false;
        public  bool CheckForRepeatsBtnIsChecked { get; set; } = false;
        public  bool FindDemoPhrasesBtnIsChecked { get; set; } = false;

        public List<string> SelectedActors { get; set; } = [];

    }
}
