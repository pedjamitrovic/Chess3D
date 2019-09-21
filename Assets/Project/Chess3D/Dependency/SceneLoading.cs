namespace Chess3D.Dependency
{
	public static class SceneLoading
	{
        public static InjectionContext Context { get; } = new InjectionContext();

        public static class Parameters
		{
			public static readonly string WhiteChoice = "#WHITE_CHOICE";
			public static readonly string BlackChoice = "#BLACK_CHOICE";
			public static readonly string WhiteDepth = "#WHITE_DEPTH";
			public static readonly string BlackDepth = "#BLACK_DEPTH";
			public static readonly string Fen = "#FEN";
            public static readonly string Board = "#BOARD";
		}
	}
}
