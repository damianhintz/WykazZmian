namespace WykazZmian.Domena
{
    public abstract class ObiektPowierzchniowy
    {
        private Powierzchnia _powierzchnia;
        
        protected ObiektPowierzchniowy(Powierzchnia powierzchnia)
        {
            _powierzchnia = powierzchnia;
        }

        public Powierzchnia powierzchnia()
        {
            return _powierzchnia;
        }

        public void powierzchnia(Powierzchnia powierzchnia)
        {
            _powierzchnia = powierzchnia;
        }

    }
}
