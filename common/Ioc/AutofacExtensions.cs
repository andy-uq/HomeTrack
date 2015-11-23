using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Builder;

namespace HomeTrack.Ioc
{
	public static class AutofacExtensions
	{
		public static IEnumerable<T> ResolveAll<T>(this IComponentContext componentContext)
		{
			return componentContext.Resolve<IEnumerable<T>>();
		}

		public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithTypedParameter<TLimit, TReflectionActivatorData, TStyle>(
			this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
			Type parameterType,
			Func<IComponentContext, object> resolveParameter)
			where TReflectionActivatorData : ReflectionActivatorData
		{
			return registration.WithParameter((p, r) => p.ParameterType == parameterType, (p, r) => resolveParameter(r));
		}

		public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithNamedParameter<TLimit, TReflectionActivatorData, TStyle>(
			this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
			string parameterName,
			Func<IComponentContext, object> resolveParameter)
			where TReflectionActivatorData : ReflectionActivatorData
		{
			return registration.WithParameter((p, r) => p.Name == parameterName, (p, r) => resolveParameter(r));
		}
	}
}