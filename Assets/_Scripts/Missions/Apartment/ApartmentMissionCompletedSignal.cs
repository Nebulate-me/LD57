namespace _Scripts.Missions.Apartment
{
    public struct ApartmentMissionCompletedSignal
    {
        public ApartmentMissionDto Dto { get; }
        public int Score { get; }
        
        public ApartmentMissionCompletedSignal(ApartmentMissionDto apartmentMissionDto, int score)
        {
            Dto = apartmentMissionDto;
            Score = score;
        }

        
    }
}