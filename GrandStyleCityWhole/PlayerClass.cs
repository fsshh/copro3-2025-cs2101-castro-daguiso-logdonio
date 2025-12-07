using System;
using System.Collections.Generic;

namespace GrandStyleCityWhole
{
    //Struct
    public struct PlayerStruct
    {
        public string PlayerName;
        public int GenderId; // Changed to int and renamed to Id
        public int HairId;
        public int HairCustomizationId;
        public int HairColorId;
        public int FaceShapeId;
        public int NoseShapeId;
        public int EyeColorId;
        public int SkinToneId;
        public int BodyTypeId;
        public int TopAttireId;

        // The lists now store the Option Id (int) for multiple accessories
        public List<int> EarringsList;
        public List<int> NecklacesList;
        public List<int> BraceletsList;
        public List<int> RingsList;

        public int ShoesId;
        public int ShoeColorId;
        public int PoseId;
        public int VideoModeId;
        public int BackgroundId;
        public int PetId;
        public int WalkAnimationId;
        public string SaveDate;


        public void InitializeLists()
        {
            EarringsList = new List<int>();
            NecklacesList = new List<int>();
            BraceletsList = new List<int>();
            RingsList = new List<int>();
        }
    }
}