using System;
using System.Collections.Generic;
using System.Reflection;
using DependencyViewer.Common.Loaders;
using Rhino.Mocks;

namespace DependencyViewer.Tests.Model
{
    public static class Helper
    {
        public static IProject GetMockProjectLoader(Guid projectIdentifier, Guid projectReference)
        {
            var projectLoader = MockRepository.GenerateStub<IProject>();
            projectLoader.Stub(p => p.Name).Return("Name");
            projectLoader.Stub(p => p.ProjectIdentifier).Return(projectIdentifier);
            projectLoader.Stub(p => p.ProjectReferences).Return(new List<Guid> { projectReference });
            projectLoader.Stub(p => p.ReferencedDlls).Return(new List<AssemblyName> { new AssemblyName("mscorlib") });
            return projectLoader;
        }
    }
}