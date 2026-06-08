namespace AttendanceDomain.Attendance.Features;

public static class AttendanceGeo
{
    private const double EarthRadiusMeters = 6371000;

    public static double DistanceMeters(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        var dLat = ToRadians(latitude2 - latitude1);
        var dLon = ToRadians(longitude2 - longitude1);
        var lat1 = ToRadians(latitude1);
        var lat2 = ToRadians(latitude2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
