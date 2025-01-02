namespace SimpleGridFly.JMXFiles.BSR
{
    public struct CPrimAniTypeData
    {
        #region Fields

        public AnimationType animationType;
        public uint animationFileIndex;
        public uint animationEventCount;
        public List<AnimationEvent> AnimationEventList;
        public uint walkPointCnt;
        public List<float[]> WalkGraphPointList;
        public float walkLength;

        #endregion Fields
    }
}