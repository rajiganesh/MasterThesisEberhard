using Autofac;
using EBS3000Dashboard.Interface;

namespace EBS3000Dashboard.Vis.Services
{
  public class ProductionServices : IProductionServices
  {
    private IDashboard _context;

    public ProductionServices()
    {
      _context = App.Container.Resolve<IDashboard>();
    }

    /// <summary>
    /// Gets the context an instance of a "DashoboardServices" object
    /// </summary>
    /// <value>
    /// The context.
    /// </value>
    public IDashboard Context =>
      _context;
  }
}
