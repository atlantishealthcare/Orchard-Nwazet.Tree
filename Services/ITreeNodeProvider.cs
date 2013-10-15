using System.Collections.Generic;
using Orchard;

namespace Nwazet.Tree.Services {
    public interface ITreeNodeProvider : IDependency {
        IEnumerable<TreeNode> GetChildren(string nodeType, string nodeId);
        TreeNode Get(string nodeType, string nodeId);
    }
}
