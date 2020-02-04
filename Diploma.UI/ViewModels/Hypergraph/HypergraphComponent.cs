namespace Diploma.UI.ViewModels.Hypergraph
{

    public class HypergraphComponent
    {

        private HypergraphViewModel _hypergraphViewModel;

        public HypergraphViewModel HypergraphViewModel
        {
            get =>
                _hypergraphViewModel;

            set
            {
                if (_hypergraphViewModel != null)
                {
                    return;
                }
                _hypergraphViewModel = value;
            }
        }

    }

}