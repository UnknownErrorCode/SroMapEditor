﻿namespace SimpleGridFly.JMXFiles.BSR
{
    public enum AnimationType : uint
    {
        POSE = 0x3C,

        STAND1 = 0x00,

        //STAND2 = 0x08,
        STAND2 = 0x7A,

        STAND3 = 0x3D,
        STAND4 = 0x51,

        ATTREADY = 0x06,

        TURN_L = 0x18,
        TURN_R = 0x19,

        SIT_DOWN = 0x0D,
        SIT = 0x0E,
        STAND_UP = 0x0F,

        DEFENCE = 0x16,

        WALK = 0x01,
        WALK_BACK = 0x17,
        RUN = 0x07,

        ATTACK1 = 0x02,
        ATTACK2 = 0x05,
        ATTACK3 = 0x10,
        ATTACK4 = 0x11,
        ATTACK5 = 0xB7,
        ATTACK6 = 0xB8,
        ATTACK7 = 0xB9,
        ATTACK8 = 0xBA,
        ATTACK9 = 0xBE,

        REVOLUTION = 0x27,

        SKILL_1 = 0x1A,
        SKILL_2 = 0x1B,
        SKILL_3 = 0x1C,
        SKILL_4 = 0x1D,
        SKILL_5 = 0x1E,
        SKILL_6 = 0x1F,
        SKILL_7 = 0x20,
        SKILL_8 = 0x21,
        SKILL_9 = 0x22,
        SKILL_10 = 0x23,

        SKILL_11 = 0x44,
        SKILL_12 = 0x45,
        SKILL_13 = 0x46,
        SKILL_14 = 0x47,
        SKILL_15 = 0x48,
        SKILL_16 = 0x49,
        SKILL_17 = 0x4A,
        SKILL_18 = 0x4B,
        SKILL_19 = 0x4C,
        SKILL_20 = 0x4D,

        SKILL_21 = 0x65,
        SKILL_22 = 0x66,
        SKILL_23 = 0x67,
        SKILL_24 = 0x68,
        SKILL_25 = 0x69,
        SKILL_26 = 0x6A,
        SKILL_27 = 0x6B,
        SKILL_28 = 0x6C,
        SKILL_29 = 0x6D,
        SKILL_30 = 0x6E,
        SKILL_31 = 0x6F,
        SKILL_32 = 0x70,
        SKILL_33 = 0x71,
        SKILL_34 = 0x72,
        SKILL_35 = 0x73,
        SKILL_36 = 0x74,
        SKILL_37 = 0x75,
        SKILL_38 = 0x76,
        SKILL_39 = 0x77,
        SKILL_40 = 0x78,

        SKILL_41 = 0x7B,
        SKILL_42 = 0x7C,
        SKILL_43 = 0x7D,
        SKILL_44 = 0x7E,
        SKILL_45 = 0x7F,
        SKILL_46 = 0x80,
        SKILL_47 = 0x81,
        SKILL_48 = 0x82,
        SKILL_49 = 0x83,
        SKILL_50 = 0x84,
        SKILL_51 = 0x85,
        SKILL_52 = 0x86,
        SKILL_53 = 0x87,
        SKILL_54 = 0x88,
        SKILL_55 = 0x89,
        SKILL_56 = 0x8A,
        SKILL_57 = 0x8B,
        SKILL_58 = 0x8C,
        SKILL_59 = 0x8D,
        SKILL_60 = 0x8E,
        SKILL_61 = 0x8F,
        SKILL_62 = 0x90,
        SKILL_63 = 0x91,
        SKILL_64 = 0x92,
        SKILL_65 = 0x93,
        SKILL_66 = 0x94,
        SKILL_67 = 0x95,
        SKILL_68 = 0x96,
        SKILL_69 = 0x97,
        SKILL_70 = 0x98,
        SKILL_71 = 0x99,
        SKILL_72 = 0x9A,
        SKILL_73 = 0x9B,
        SKILL_74 = 0x9C,
        SKILL_75 = 0x9D,
        SKILL_76 = 0x9E,
        SKILL_77 = 0x9F,
        SKILL_78 = 0xA0,
        SKILL_79 = 0xA1,
        SKILL_80 = 0xA2,
        SKILL_81 = 0xA3,
        SKILL_82 = 0xA4,
        SKILL_83 = 0xA5,
        SKILL_84 = 0xA6,
        SKILL_85 = 0xA7,
        SKILL_86 = 0xA8,
        SKILL_87 = 0xA9,
        SKILL_88 = 0xAA,
        SKILL_89 = 0xAB,
        SKILL_90 = 0xAC,
        SKILL_91 = 0xAD,
        SKILL_92 = 0xAE,
        SKILL_93 = 0xAF,
        SKILL_94 = 0xB0,
        SKILL_95 = 0xB1,
        SKILL_96 = 0xB2,
        SKILL_97 = 0xB3,
        SKILL_98 = 0xB4,
        SKILL_99 = 0xB5,
        SKILL_100 = 0xB6,

        READY01 = 0x28,
        READY02 = 0x29,
        READY03 = 0x2A,
        READY04 = 0x2B,
        READY05 = 0x2C,

        WAIT01 = 0x5B,
        WAIT02 = 0x5C,
        WAIT03 = 0x5D,
        WAIT04 = 0x5E,
        WAIT05 = 0x5F,

        HAMMER = 0xBB,
        HANDLOOF = 0xBC,
        TROW = 0xBD,
        MG_SSELF = 0x13,
        MG_SOTHER = 0x14,
        DAMAGE1 = 0x03,
        DAMAGE2 = 0x09,
        HELP = 0x43,
        FIND = 0x4E,
        STUN = 0x4F,

        DIE1 = 0x04,
        DIE1_RM = 0x24,
        DIE2 = 0x12,
        DIE2_RM = 0x25,

        REVIVAL = 0x79,

        DOWN = 0x3E,
        DOWN_RM = 0x3F,
        DOWN_DAMAGE = 0x40,
        DOWN_UP = 0x41,
        DOWN_DIE = 0x42,

        PICK = 0x26,
        CLICK = 0x0A,

        CB_YEONHWAN = 0x0B,
        CB_2 = 0x0C,

        ET_BYE = 0x15,

        EMOTION01 = 0x32,
        EMOTION02 = 0x33,
        EMOTION03 = 0x34,
        EMOTION04 = 0x35,
        EMOTION05 = 0x36,
        EMOTION06 = 0x37,
        EMOTION07 = 0x38,
        EMOTION08 = 0x39,
        EMOTION09 = 0x3A,
        EMOTION10 = 0x3B,

        VENDOR01 = 0x50,

        SHOT = 0xBF,    //not implemented in vSRO?
    }
}