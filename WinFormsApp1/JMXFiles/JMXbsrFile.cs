using OpenTK.Mathematics;
using SimpleGridFly.JMXFiles.BSR;
using WinFormsApp1;

namespace SimpleGridFly.JMXFiles
{
    public class JMXbsrFile
    {
        #region Fields

        /// <summary>
        /// Number of Animations inside the bsr.
        /// </summary>
        public readonly uint animationCnt;

        /// <summary>
        /// JMXVRES 0109
        /// </summary>
        private JMXHeader header;

        /// <summary>
        /// MATERIALSET_MAXCOUNTMAX = 5.
        /// </summary>
        public uint mtrlSetCnt;

        /// <summary>
        /// Number of Meshes contained in the bsr file.
        /// </summary>
        public uint meshCnt;

        public float[] collisionBox0;

        //24
        public float[] collisionBox1;

        //if(requireCollisionMatrix<>0)
        public byte[] collisionMatrix;

        public string collisionMesh;
        public uint collisionMeshLength;

        //24
        public uint requireCollisionMatrix;

        private readonly uint pointerMaterial;
        private readonly uint pointerMesh;
        private readonly uint pointerSkeleton;
        private readonly uint pointerAnimation;
        private readonly uint pointerPrimMeshGroup;
        private readonly uint pointerPrimAniGroup;
        private readonly uint pointerModPalette;
        private readonly uint pointerCollision;
        private readonly uint int0;
        private readonly uint int1;
        private readonly uint int2;
        private readonly uint int3;
        private readonly uint int4;

        /// <summary>
        /// reserved  40 length
        /// </summary>
        private readonly byte[] unkBuffer0;

        /// <summary>
        /// if has Skeleton CPrimBranch gets read.
        /// </summary>
        private readonly uint hasSkeleton;

        /// <summary>
        /// CPrimBranch
        /// </summary>
        private readonly uint skeletonPathLength;

        /// <summary>
        /// CPrimBranch
        /// </summary>
        private readonly string skeletonPath;

        /// <summary>
        /// CPrimBranch
        /// </summary>
        private readonly uint attachmentBoneLength;

        /// <summary>
        /// CPrimBranch
        /// </summary>
        private readonly string attachmentBone;

        /// <summary>
        /// Number of Animation Groups.
        /// </summary>
        private readonly uint aniGroupCnt;

        /// <summary>
        /// Animation Groups.
        /// </summary>
        private readonly List<CPrimAniGroup> aniGroupList;

        /// <summary>
        /// Number of Mesh Groups.
        /// </summary>
        private readonly uint meshGroupCnt;

        /// <summary>
        /// Mesh Groups.
        /// </summary>
        private readonly List<CPrimMeshGroup> meshGroupList;

        /// <summary>
        /// Number of ModelSets.
        /// </summary>
        private readonly uint modSetCnt;

        /// <summary>
        /// Model Data Sets.
        /// </summary>
        private readonly List<CModDataSet> modSetDataSetList;

        /// <summary>
        /// General information about the bsr object.
        /// </summary>
        private CObjectInfo generalinfo;

        /// <summary>
        /// Material Sets of bsr file.
        /// </summary>
        private List<CPrimMaterialSet> materialSets;

        /// <summary>
        /// CPrimMeshes inside the bsr file.
        /// </summary>
        private List<CPrimMeshValuePair> meshArray;

        /// <summary>
        /// 0, "User Define Animation Type을 사용 하였습니다."
        /// </summary>
        private uint animationTypeUserDefine;

        /// <summary>
        ///  ANIMATION_TOOL_VERSION = 0x1000, "Animation Type의 Version이 다릅니다."
        /// </summary>
        private uint animationTypeVersion;

        /// <summary>
        /// Animation names.
        /// </summary>
        private List<string> cPrimAnimation;

        public JMXbsrFile(byte[] file)
        {
            try
            {
                using (Stream s = new MemoryStream(file))
                {
                    using (BinaryReader reader = new BinaryReader(s))
                    {
                        header = new JMXHeader(reader.ReadChars(12), JmxFileFormat._bsr);

                        pointerMaterial = reader.ReadUInt32();
                        pointerMesh = reader.ReadUInt32();
                        pointerSkeleton = reader.ReadUInt32();
                        pointerAnimation = reader.ReadUInt32();
                        pointerPrimMeshGroup = reader.ReadUInt32();
                        pointerPrimAniGroup = reader.ReadUInt32();
                        pointerModPalette = reader.ReadUInt32();
                        pointerCollision = reader.ReadUInt32();
                        int0 = reader.ReadUInt32();
                        int1 = reader.ReadUInt32();
                        int2 = reader.ReadUInt32();
                        int3 = reader.ReadUInt32();
                        int4 = reader.ReadUInt32();

                        Generalinfo = new CObjectInfo()
                        {
                            Type = (ResourceType)reader.ReadUInt32(),
                            Name = new string(reader.ReadChars((int)reader.ReadUInt32())),
                            UnkUInt0 = reader.ReadUInt32(),
                            UnkUInt1 = reader.ReadUInt32()
                        };
                        unkBuffer0 = reader.ReadBytes(40);

                        #region PointerMaterial

                        reader.BaseStream.Position = pointerMaterial;
                        mtrlSetCnt = reader.ReadUInt32();
                        MaterialSets = new List<CPrimMaterialSet>((int)mtrlSetCnt);
                        for (int mtrlSetIndex = 0; mtrlSetIndex < mtrlSetCnt; mtrlSetIndex++)
                        {
                            CPrimMaterialSet mtrlSet = new CPrimMaterialSet()
                            {
                                MaterialID = reader.ReadUInt32(),
                                MaterialPath = new string(reader.ReadChars(reader.ReadInt32())),
                            };
                            MaterialSets.Add(mtrlSet);
                        }

                        PointerMaterialEnd = reader.BaseStream.Position;

                        #endregion PointerMaterial

                        #region Mesh

                        reader.BaseStream.Position = pointerMesh;

                        meshCnt = reader.ReadUInt32();
                        MeshArray = new List<CPrimMeshValuePair>((int)meshCnt);
                        for (int meshIndex = 0; meshIndex < meshCnt; meshIndex++)
                        {
                            var mesh = new CPrimMeshValuePair()
                            {
                                MeshPath = new string(reader.ReadChars((int)reader.ReadUInt32()))
                            };

                            mesh.MeshUnkUInt0 = GroupMeshPairValues == 1 ? reader.ReadUInt32() : 0;

                            MeshArray.Add(mesh);
                        }
                        PointerMeshEnd = reader.BaseStream.Position;

                        #endregion Mesh

                        #region Skeleton

                        reader.BaseStream.Position = pointerSkeleton;
                        hasSkeleton = reader.ReadUInt32();
                        if (hasSkeleton == 0x01)
                        {
                            skeletonPathLength = reader.ReadUInt32();
                            skeletonPath = new string(reader.ReadChars((int)skeletonPathLength));

                            attachmentBoneLength = reader.ReadUInt32();
                            attachmentBone = new string(reader.ReadChars((int)attachmentBoneLength));
                        }
                        PointerSkeletonEnd = reader.BaseStream.Position;

                        #endregion Skeleton

                        #region Animation

                        reader.BaseStream.Position = pointerAnimation;
                        AnimationTypeVersion = reader.ReadUInt32();
                        AnimationTypeUserDefine = reader.ReadUInt32();
                        animationCnt = reader.ReadUInt32();
                        CPrimAnimation = new List<string>((int)animationCnt);
                        for (int animationIndex = 0; animationIndex < animationCnt; animationIndex++)
                        {
                            CPrimAnimation.Add(new string(reader.ReadChars((int)reader.ReadUInt32())));
                        }
                        PointerAnimationEnd = reader.BaseStream.Position;

                        #endregion Animation

                        #region PrimMeshGroup

                        reader.BaseStream.Position = pointerPrimMeshGroup;
                        meshGroupCnt = reader.ReadUInt32();
                        meshGroupList = new List<CPrimMeshGroup>((int)meshGroupCnt);
                        for (int meshGroupIndex = 0; meshGroupIndex < meshGroupCnt; meshGroupIndex++)
                        {
                            CPrimMeshGroup meshGroup = new CPrimMeshGroup()
                            {
                                meshGroupName = new string(reader.ReadChars((int)reader.ReadUInt32())),
                                meshFileCnt = reader.ReadUInt32()
                            };
                            meshGroup.MeshGroupIndex = new uint[meshGroup.meshFileCnt];
                            for (int meshGroupFileIndex = 0; meshGroupFileIndex < meshGroup.meshFileCnt; meshGroupFileIndex++)
                            {
                                meshGroup.MeshGroupIndex[meshGroupFileIndex] = reader.ReadUInt32();
                            }
                            meshGroupList.Add(meshGroup);
                        }
                        PointerPrimMeshGroupEnd = reader.BaseStream.Position;

                        #endregion PrimMeshGroup

                        #region PrimAniGroup

                        reader.BaseStream.Position = pointerPrimAniGroup;
                        aniGroupCnt = reader.ReadUInt32();
                        aniGroupList = new List<CPrimAniGroup>((int)aniGroupCnt);
                        for (int aniGroupIndex = 0; aniGroupIndex < aniGroupCnt; aniGroupIndex++)
                        {
                            CPrimAniGroup aniGroup = new CPrimAniGroup()
                            {
                                AnimationGroupName = new string(reader.ReadChars((int)reader.ReadUInt32())),
                                AnimationTypeDataCount = reader.ReadUInt32()
                            };
                            aniGroup.PrimAniTypeDataList = new List<CPrimAniTypeData>((int)aniGroup.AnimationTypeDataCount);
                            for (int aniTypeDataIndex = 0; aniTypeDataIndex < aniGroup.AnimationTypeDataCount; aniTypeDataIndex++)
                            {
                                CPrimAniTypeData aniTypeData = new CPrimAniTypeData()
                                {
                                    animationType = (AnimationType)reader.ReadUInt32(),
                                    animationFileIndex = reader.ReadUInt32(),
                                    animationEventCount = reader.ReadUInt32()
                                };
                                aniTypeData.AnimationEventList = new List<AnimationEvent>((int)aniTypeData.animationEventCount);
                                for (int animationEvent = 0; animationEvent < aniTypeData.animationEventCount; animationEvent++)
                                {
                                    AnimationEvent aniEvent = new AnimationEvent()
                                    {
                                        KeyTime = reader.ReadUInt32(),
                                        Type = reader.ReadUInt32(),
                                        unkValue0 = reader.ReadUInt32(),
                                        unkValue1 = reader.ReadUInt32()
                                    };
                                    aniTypeData.AnimationEventList.Add(aniEvent);
                                }
                                aniTypeData.walkPointCnt = reader.ReadUInt32();
                                aniTypeData.walkLength = reader.ReadUInt32();
                                aniTypeData.WalkGraphPointList = new List<float[]>((int)aniTypeData.walkPointCnt);
                                for (int walkPointIndex = 0; walkPointIndex < aniTypeData.walkPointCnt; walkPointIndex++)
                                {
                                    aniTypeData.WalkGraphPointList.Add(new float[2] { reader.ReadSingle(), reader.ReadSingle() });
                                }

                                aniGroup.PrimAniTypeDataList.Add(aniTypeData);
                            }
                            aniGroupList.Add(aniGroup);
                        }
                        PointerPrimAniGroupEnd = reader.BaseStream.Position;

                        #endregion PrimAniGroup

                        #region ModPalette

                        reader.BaseStream.Position = pointerModPalette;

                        modSetCnt = reader.ReadUInt32();
                        modSetDataSetList = new List<CModDataSet>((int)modSetCnt);
                        for (int modSetIndex = 0; modSetIndex < modSetCnt; modSetIndex++)
                        {
                            CModDataSet modSetData = new CModDataSet()
                            {
                                Type = reader.ReadUInt32(),
                                AniType = (AnimationType)reader.ReadUInt32(),
                                Name = new string(reader.ReadChars((int)reader.ReadUInt32())),
                                modSetDataCnt = reader.ReadUInt32()
                            };
                            modSetData.ModDataTypes = new List<IModData>((int)modSetData.modSetDataCnt);
                            for (int modDataTypeIndex = 0; modDataTypeIndex < modSetData.modSetDataCnt; modDataTypeIndex++)
                            {
                                ModDataType type = (ModDataType)reader.ReadUInt32();
                                switch (type)
                                {
                                    case ModDataType.ModDataMtrl:
                                        modSetData.ModDataTypes.Add((CModDataMaterial)GetModData(type, reader));
                                        break;

                                    case ModDataType.ModDataTexAni:
                                        break;

                                    case ModDataType.ModDataMultiTex:
                                        break;

                                    case ModDataType.ModDataMultiTexRev:
                                        break;

                                    case ModDataType.ModDataParticle:
                                        modSetData.ModDataTypes.Add((CModDataParticle)GetModData(type, reader));
                                        break;

                                    case ModDataType.ModDataEnvMap:
                                        break;

                                    case ModDataType.ModDataBumpEnv:
                                        break;

                                    case ModDataType.ModDataSound:
                                        break;

                                    case ModDataType.ModDataDyVertex:
                                        break;

                                    case ModDataType.ModDataDyJoint:
                                        break;

                                    case ModDataType.ModDataDyLattice:
                                        break;

                                    case ModDataType.ModDataProgEquipPow:
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }

                        PointerModPaletteEnd = reader.BaseStream.Position;

                        #endregion ModPalette

                        reader.BaseStream.Position = pointerCollision;
                        PointerCollisionEnd = reader.BaseStream.Position;
                    }
                }
                Initialized = true;
            }
            catch (System.Exception e)
            {
                Initialized = false;
            }
        }

        public uint PointerCollision => pointerCollision;

        public long PointerCollisionEnd { get; }

        public uint PointerMaterial => pointerMaterial;

        public long PointerMaterialEnd { get; }

        public uint PointerMesh => pointerMesh;

        public long PointerMeshEnd { get; }

        public uint PointerAnimation => pointerAnimation;

        public long PointerAnimationEnd { get; }

        public uint PointerSkeleton => pointerSkeleton;

        public long PointerSkeletonEnd { get; }

        public uint PointerPrimMeshGroup => pointerPrimMeshGroup;

        public long PointerPrimMeshGroupEnd { get; }

        public uint PointerPrimAniGroup => pointerPrimAniGroup;

        public long PointerPrimAniGroupEnd { get; }

        public uint PointerModPalette => pointerModPalette;

        public long PointerModPaletteEnd { get; }

        public CObjectInfo Generalinfo { get => generalinfo; set => generalinfo = value; }

        public byte[] UnkBuffer0 { get => unkBuffer0; }

        public uint GroupMeshPairValues { get => int0; }

        // ??
        public uint Int1 { get => int1; }

        public uint Int2 { get => int2; }

        public uint Int3 { get => int3; }

        public uint Int4 { get => int4; }

        public List<CPrimMaterialSet> MaterialSets { get => materialSets; set => materialSets = value; }

        public List<CPrimMeshValuePair> MeshArray { get => meshArray; set => meshArray = value; }

        public uint HasSkeleton => hasSkeleton;

        public string SkeletonPath => skeletonPath;

        public string AttachmentBone => attachmentBone;

        public uint AnimationTypeUserDefine { get => animationTypeUserDefine; set => animationTypeUserDefine = value; }

        public uint AnimationTypeVersion { get => animationTypeVersion; set => animationTypeVersion = value; }

        public List<string> CPrimAnimation { get => cPrimAnimation; set => cPrimAnimation = value; }

        public List<CPrimAniGroup> AniGroupList => aniGroupList;

        public List<CPrimMeshGroup> MeshGroupList => meshGroupList;

        public List<CModDataSet> ModSetDataSetList => modSetDataSetList;

        public string Name => header.Value;

        public bool Initialized { get; }
        public JMXHeader Header => header;

        #endregion Fields

        private IModData GetModData(ModDataType type, BinaryReader reader)
        {
            CModData data = new CModData();
            data.Type = type;
            data.Float0 = reader.ReadSingle();
            data.Int0 = reader.ReadInt32();
            data.Int1 = reader.ReadInt32();
            data.Int2 = reader.ReadInt32();
            data.Int3 = reader.ReadInt32();
            data.Int4 = reader.ReadInt32();
            data.Byte0 = reader.ReadByte();
            data.Byte1 = reader.ReadByte();
            data.Byte2 = reader.ReadByte();
            data.Byte3 = reader.ReadByte();

            switch (type)
            {
                case ModDataType.ModDataMtrl:
                    CModDataMaterial data1 = new CModDataMaterial(data);
                    data1.unkUInt5 = reader.ReadUInt32();
                    data1.unkUInt6 = reader.ReadUInt32();
                    data1.unkUInt7 = reader.ReadUInt32();
                    data1.gradientKeyCount = reader.ReadUInt32();
                    data1.GradientKeys = new List<GradientKey>((int)data1.gradientKeyCount);
                    for (int gKeys = 0; gKeys < data1.gradientKeyCount; gKeys++)
                    {
                        var gkey = new GradientKey()
                        {
                            Time = reader.ReadUInt32(),
                            Values = new List<float>(4)
                        };
                        for (int color4 = 0; color4 < 4; color4++)
                        {
                            gkey.Values.Add(reader.ReadSingle());
                        }
                        data1.GradientKeys.Add(gkey);
                    }
                    if (data1.unkUInt6 == 4)
                    {
                        data1.curvedKeyCount = reader.ReadUInt32();
                        data1.CurvedKeys = new List<CurvedKey>((int)data1.curvedKeyCount);
                        for (int i = 0; i < data1.curvedKeyCount; i++)
                        {
                            data1.CurvedKeys.Add(new CurvedKey()
                            {
                                Time = reader.ReadUInt32(),
                                Value = reader.ReadSingle()
                            });
                        }
                    }
                    data1.unkUInt8 = reader.ReadUInt32();
                    data1.unkUInt9 = reader.ReadUInt32();
                    data1.unkUInt10 = reader.ReadUInt32();
                    data1.unkUInt11 = reader.ReadUInt32();
                    data1.unkUInt12 = reader.ReadUInt32();
                    data1.unkUInt13 = reader.ReadUInt32();
                    data1.unkUInt14 = reader.ReadUInt32();
                    data1.unkUInt15 = reader.ReadUInt32();
                    data1.unkUInt16 = reader.ReadUInt32();

                    break;

                case ModDataType.ModDataTexAni:
                    return null;

                case ModDataType.ModDataMultiTex:
                    CModDataMultiTexture data2 = new CModDataMultiTexture(data);
                    data2.unkUInt5 = reader.ReadUInt32();
                    data2.TextureLength = reader.ReadUInt32();
                    data2.Texture = new string(reader.ReadChars((int)data2.TextureLength));
                    data2.unkUInt6 = reader.ReadUInt32();
                    return data2;

                case ModDataType.ModDataMultiTexRev:
                    CModDataMultiTextureRev data3 = new CModDataMultiTextureRev(data);
                    data3.unkUInt5 = reader.ReadUInt32();
                    data3.TextureLength = reader.ReadUInt32();
                    data3.Texture = new string(reader.ReadChars((int)data3.TextureLength));
                    data3.unkUInt6 = reader.ReadUInt32();
                    return data3;

                case ModDataType.ModDataParticle:
                    CModDataParticle data4 = new CModDataParticle(data);
                    data4.ParticleCount = reader.ReadUInt32();
                    data4.Particles = new CModDataParticleStruct[(int)data4.ParticleCount];
                    for (int particleIndex = 0; particleIndex < data4.ParticleCount; particleIndex++)
                    {
                        CModDataParticleStruct pStruct = new CModDataParticleStruct()
                        {
                            IsEnabled = reader.ReadUInt32(),
                            FileName = new string(reader.ReadChars((int)reader.ReadUInt32())),
                            Bone = new string(reader.ReadChars((int)reader.ReadUInt32())),
                            BonePosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                            BirthTime = reader.ReadUInt32(),
                            UnkByte0 = reader.ReadByte(),
                            UnkByte1 = reader.ReadByte(),
                            UnkByte2 = reader.ReadByte(),
                            UnkByte3 = reader.ReadByte(),
                        };
                        pStruct.UnkVector = pStruct.UnkByte3 == 1 ? new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()) : new Vector3();
                    }
                    return data4;

                case ModDataType.ModDataEnvMap:
                    break;

                case ModDataType.ModDataBumpEnv:
                    break;

                case ModDataType.ModDataSound:
                    break;

                case ModDataType.ModDataDyVertex:
                    break;

                case ModDataType.ModDataDyJoint:
                    break;

                case ModDataType.ModDataDyLattice:
                    break;

                case ModDataType.ModDataProgEquipPow:
                    break;

                default:

                    break;
            }

            return null;
        }
    }
}