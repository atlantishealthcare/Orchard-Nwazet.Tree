using System.Collections.Generic;

namespace Nwazet.Tree.ViewModels {
    public class TreeExplorerViewModel {
        public TreeNode Node { get; set; }
        public IEnumerable<TreeNode> Children { get; set; }
    }
}