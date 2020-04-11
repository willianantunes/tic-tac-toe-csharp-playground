namespace src.Helper
{
    public static class ObjectExtensionMethods
    {
        public static bool IsNotNull(this object targetObject)
        {
            return targetObject != null;
        }
        
        public static bool IsNull(this object targetObject)
        {
            return targetObject == null;
        }
    }
}