using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シーン間で共有したい定数を宣言するクラス

namespace Common
{
    
	public static class Define
	{

		public const float DOUBLETAPTIME = 0.30f;

		public const float CAMERA_SIZE = 50.0f;

		public const float CANBUS_HEIGHT = 1300.0f;
		public const float CANBUS_WIDTH = 1125.0f;

		public const float ITEMICON_IDEALSIZE = 182.83333f;
		public const float ITEMICON_MARGIN = 4.0f;
		public const float ITEMVIEW_IDEALSIZE = 800.0f;
		public const float ITEMICON_SCROLLBARHEIGHT = 28.0f;
		//direction
        public const int DOWN = 0;
        public const int LEFT = 1;
        public const int UP = 2;
        public const int RIGHT = 3;
		//wallNo
        public const int WALL_LEFT = 1;
        public const int WALL_FRONT = 2;
        public const int WALL_RIGHT = 3;
        public const int WALL_BACK = 4;
		//PaintsCorner
		public const int CORNER_UPPERLEFT = 0;
		public const int CORNER_UPPERRIGHT = 1;
		public const int CORNER_LOWERRIGHT = 2;
		public const int CORNER_LOWERLEFT = 3;
		//soundsClipNo
        public const string SOUND_DOOROPEN1 = "door-open1";
        public const string SOUND_DRAWEROPEN1 = "drawer-open1";
        public const string SOUND_DECISION22 = "decision22";
        public const string SOUND_DECISION4 = "decision4";
        public const string SOUND_KEY_IN2 = "key-in2";
        public const string SOUND_CURSOR7 = "cursor7";
        public const string SOUND_CURSOR1 = "cursor1";
        public const string SOUND_SHAKASHAKA = "shakashaka01";
        public const string SOUND_KEZURI = "kezuri01";
        public const string SOUND_DUKTWCHIN = "duktwchin";
        public const string SOUND_KACHAN05 = "kachan05";
        public const string SOUND_CANTOPEN02 = "cantOpen02";
        public const string SOUND_CANTOPEN03 = "cantOpen03";
        public const string SOUND_CANTOPEN04 = "cantOpen04";
        public const string SOUND_FURNITURECANTOPEN01 = "furnitureCantOpen01";
        public const string SOUND_FURNITURECANTOPEN02 = "furnitureCantOpen02";
        public const string SOUND_FURNITURECANTOPEN03 = "furnitureCantOpen03";
        public const string SOUND_FURNITURECANTOPEN04 = "furnitureCantOpen04";
        public const string SOUND_DRAWER01 = "drawer01";
        public const string SOUND_DRAWER04 = "drawer04";
        public const string SOUND_DRAWER06 = "drawer06";
        public const string SOUND_DRAWER07 = "drawer07";
        public const string SOUND_DRAWER08 = "drawer08";
        public const string SOUND_DRAWER09 = "drawer09";
        public const string SOUND_DRAWER11 = "drawer11";
        public const string SOUND_DRAWER14 = "drawer14";

        public const string SOUND_DRAWERCLOSE02 = "drawerClose02";
        public const string SOUND_DRAWERCLOSE05 = "drawerClose05";
        public const string SOUND_DRAWERCLOSE06 = "drawerClose05";
        public const string SOUND_DRAWERCLOSE15 = "drawerClose15";
        public const string SOUND_DRAWERCLOSE19 = "drawerClose19";

        public const string SOUND_GOTON01 = "goton01";
        public const string SOUND_PATA01 = "pata01";
        public const string SOUND_HUWHUWHLLL = "huwhuwhlll";
        public const string SOUND_SWITCH04 = "switch04";
		public const string SOUND_PI01 = "pi01";
		public const string SOUND_PI03 = "pi03";
		public const string SOUND_FURNITUREDOOROPEN04 = "furnitureDoorOpen04";
		public const string SOUND_FURNITUREDOORCLOSE02 = "furnitureDoorClose02";
		public const string SOUND_SCISSORS02 = "Scissors02";
		public const string SOUND_GLASS01 = "Glass01";
		public const string SOUND_GLASS02 = "Glass02";
		public const string SOUND_GLASS08 = "Glass08";
		public const string SOUND_GLASS09 = "Glass09";
		public const string SOUND_SHAAAA = "shaaaaa";
		public const string SOUND_KTON01 = "kTon01";
		public const string SOUND_KTON07 = "kTon07";
		public const string SOUND_TOKO01 = "toko01";
			

		public const string SOUND_AVOID = "avoid";

		//item
		public const string ITEM_GINGER1 = "Ginger1";
		public const string ITEM_GINGER2 = "Ginger2";
		public const string ITEM_TWOGINGERS = "TwoGingers";
		public const string ITEM_OSARA = "Osara";
		public const string ITEM_MONKO = "Monko";
		public const string ITEM_BANANA = "Banana";
		public const string ITEM_BANANA2 = "Banana2";
		public const string ITEM_FLOWER = "Flower";
		public const string ITEM_FLOWERYELLOW = "FlowerYellow";
		public const string ITEM_SCISSORS = "Scissors";
		public const string ITEM_CUTFLOWER = "CutFlower";
		public const string ITEM_CUTFLOWERYELLOW = "CutFlowerYellow";
		public const string ITEM_PENCIL = "Pencil";
		public const string ITEM_PENCILSHARPNER = "PencilSharpner";
		public const string ITEM_SHARPEDPENCIL = "SharpedPencil";
		public const string ITEM_PAPER = "Paper";
		public const string ITEM_PAPERDRAW = "PaperDraw";
		public const string ITEM_CUTKEY = "CutKey";
		public const string ITEM_CUTKEY1 = "CutKey1";
		public const string ITEM_CUTKEY2 = "CutKey2";
		public const string ITEM_CHAIR = "Chair";
		public const string ITEM_REDKEY = "RedKey";
		public const string ITEM_CUBEDOG = "CubeDog";
		public const string ITEM_BONE = "Bone";
		public const string ITEM_GLASSDOOR = "GlassDoor";
		public const string ITEM_GLASSTRASHBOX = "GlassTrashBox";

		//gameFlag


		public const string FLAG_PUT_BANANA = "Put_Banana";
		public const string FLAG_PUT_BANANA2 = "Put_Banana2";
		public const string FLAG_OPEN_LRBOX = "Open_LRBox";
		public const string FLAG_OPEN_DIRBOX  = "Open_DirBox";
		public const string FLAG_OPEN_STARBOX  = "Opne_StarBox";
		public const string FLAG_PUT_CHAIR  = "Put_Chair";
		public const string FLAG_OPEN_NUMBERBOX  = "Open_NumberBox";
		public const string FLAG_USE_REDKEY = "Use_RedKey";
		public const string FLAG_USE_CUTKEY = "UseCutKey";//public const string FLAG_OPENLRBOX
		public const string FLAG_PUT_CUBEDOG   = "Put_CubeDog";
		public const string FLAG_PUT_BONE   = "Put_Bone";
		public const string FLAG_PLAY_DOGVIDEO   = "Play_DogVideo";
		public const string FLAG_PUT_OSARA = "Put_Osara";
        public const string FLAG_PUT_MONKO = "Put_Monko";
		public const string FLAG_PUT_GLASSDOOR   = "Put_GlassDoor";
		public const string FLAG_PUT_GLASSTRASHBOX   = "Put_GlassTrashBox";
		public const string FLAG_OPEN_DIRECTIONPUZZULE = "Open_DirectionPuzzle";
		public const string FLAG_PUT_ONEGINGER = "Put_OneGinger";
        public const string FLAG_PUT_TWOGINGER = "Put_TwoGinger";
        public const string FLAG_CUT_FLOWER = "Cut_Flower";
        public const string FLAG_CUT_FLOWERYELLOW = "Cut_FlowerYellow";
		public const string FLAG_PUT_CUTFLOWER = "Put_CutFlower";
		public const string FLAG_PUT_CUTFLOWERYELLOW = "Put_CutFlowerYellow";
		public const string FLAG_OPENDOOR  = "OpenDoor";
		public const string FLAG_OPEN_PAINT = "Open_Paint";
		//public const string FLAG_ = "";
        //public const string FLAG_ = "";

		public const string FLAG_GET_CHAIR = "Get_Chair";
		public const string FLAG_GET_GINGER1 = "Get_Ginger1";
		public const string FLAG_GET_GINGER2 = "Get_Ginger2";
		public const string FLAG_GET_OSARA = "Get_Osara";
		public const string FLAG_GET_MONKO = "Get_Monko";
		public const string FLAG_GET_BANANA = "Get_Banana";
		public const string FLAG_GET_BANANA2 = "Get_Banana2";
		public const string FLAG_GET_FLOWER = "Get_Flower";
		public const string FLAG_GET_FLOWERYELLOW = "Get_FlowerYellow";
		public const string FLAG_GET_SCISSORS = "Get_Scissors";
		public const string FLAG_GET_CUTFLOWER = "Get_CutFlower";
		public const string FLAG_GET_CUTFLOWERYELLOW = "Get_CutFlowerYellow";
		public const string FLAG_GET_PENCIL = "Get_Pencil";
		public const string FLAG_GET_PENCILSHARPNER = "Get_PencilSharpner";
		public const string FLAG_GET_SHARPEDPENCIL = "Get_SharpedPencil";
		public const string FLAG_GET_PAPER = "Get_Paper";
		public const string FLAG_GET_PAPERDRAW = "Get_PaperDraw";
		public const string FLAG_GET_CUTKEY = "Get_CutKey";
		public const string FLAG_GET_CUTKEY1 = "Get_CutKey1";
		public const string FLAG_GET_CUTKEY2 = "Get_CutKey2";
		public const string FLAG_GET_REDKEY = "Get_RedKey";
		public const string FLAG_GET_CUBEDOG = "Get_CubeDog";
		public const string FLAG_GET_BONE = "Get_Bone";
		public const string FLAG_GET_GLASSDOOR = "Get_GlassDoor";
		public const string FLAG_GET_GLASSTRASHBOX = "Get_GlassTrashBox";
		//piece
		public const int GLASSTRASHBOX = 0;
		public const int GLASSDOG = 1;
		public const int GLASSMONKEY = 2;
		public const int GLASSDOOR = 3;

		//puzzle direction
		public const int PUZZLE_LEFT = 0;
		public const int PUZZLE_FRONT = 1;
		public const int PUZZLE_RIGHT = 2;
		public const int PUZZLE_BACK = 3;

      //  public const int 
	}

}