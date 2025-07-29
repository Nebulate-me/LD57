namespace _Scripts.Missions.Apartment
{
    public struct ApartmentMissionCompletedSignal
    {
        public ApartmentMissionDto Dto { get; }
        
        public ApartmentMissionCompletedSignal(ApartmentMissionDto apartmentMissionDto)
        {
            Dto = apartmentMissionDto;
        }
    }
}