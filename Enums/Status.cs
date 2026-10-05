namespace ServiceManagerApp;

public enum Status
{
    Accepted,     // átvéve
    Diagnosing,   // bevizsgálás alatt
    InRepair,     // javítás alatt
    Finished,     // elkészült, átvehető
    HandedOver,   // átadva az ügyfélnek
    Cancelled     // visszavonva
}