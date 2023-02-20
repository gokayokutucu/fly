using Moq;
using Moq.AutoMock;

namespace Fly.Tests.Helper;

public class TestsFor<TInstance> : IDisposable where TInstance : class
    {
        protected TInstance Instance { get; private set; }
        protected AutoMocker AutoMocker { get; private set; }
        protected Mock<TInstance> Moq { get; private set; }
        
        public TestsFor()
        {
            AutoMocker = new AutoMocker();
            BeforeInstanceCreated();
            Instance = AutoMocker.CreateInstance<TInstance>();
            Moq = AutoMocker.GetMock<TInstance>();
            AfterInstanceCreated();
        }

        /// <summary>
        /// Override this method to execute code before the Instance of the class under test is created
        /// </summary>
        public virtual void BeforeInstanceCreated()
        {
            // No ımpl. here
        }

        /// <summary>
        /// Override this method to execute code after the Instance of the class under test is created
        /// </summary>
        public virtual void AfterInstanceCreated()
        {
            // No ımpl. here
        }

        /// <summary>
        /// Use this method to inject spesific instances of a TCOnract into to automocker
        /// </summary>
        /// <typeparam name="TContract">The type for which you need to replace the automatically generated mock</typeparam>
        /// <param name="with">the concrete instance you wannt to replace the TContract with</param>
        public void Inject<TContract>(TContract with) where TContract : class
        {
            AutoMocker.Use(with);
        }

        /// <summary>
        /// Returns the object generated by the mock the given contract
        /// </summary>
        /// <typeparam name="TContract">the contract (interface) for which you want the instance </typeparam>
        /// <returns>The object instance of the TContract</returns>
        public TContract InstanceOf<TContract>() where TContract : class
        {
            return AutoMocker.Get<TContract>();
        }

        /// <summary>
        /// Returns the mock that owns the instance of TContract, in order to do preparation or verification
        /// </summary>
        /// <typeparam name="TContract">The type of dependency tahatb you need </typeparam>
        /// <returns>The mock that generated the instance of TContract</returns>
        public Mock<TContract> GetMockFor<TContract>() where TContract : class
        {
            return Mock.Get(AutoMocker.Get<TContract>());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposableInstance = Instance as IDisposable;
                if (disposableInstance != null)
                {
                    disposableInstance.Dispose();
                }
            }
        }
    }