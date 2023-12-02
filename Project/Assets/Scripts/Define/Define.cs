using UnityEngine;

public static class UnityLayer
{
        public static LayerMask Layer_Default = 1<<0;
        public static LayerMask Layer_TransparentFX = 1 << 1;
        public static LayerMask Layer_IgnoreRaycast = 1 << 2;
        public static LayerMask Layer_Water = 1 << 4;
        public static LayerMask Layer_UI = 1 << 5;
        public static LayerMask Layer_Body = 1 << 6;
}
