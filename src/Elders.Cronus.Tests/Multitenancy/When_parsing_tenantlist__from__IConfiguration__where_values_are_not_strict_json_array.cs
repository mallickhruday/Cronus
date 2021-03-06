﻿using Machine.Specifications;
using System.Linq;

namespace Elders.Cronus.Multitenancy
{
    [Subject("Tenants")]
    public class When_parsing_tenantlist__from__IConfiguration__where_values_are_not_strict_json_array
    {
        Establish context = () =>
        {
            configuration = new ConfigurationMock();
            configuration[Tenants.SettingKey] = @"tEnant1 , tEnant2";
        };

        Because of = () => tenants = new Tenants(configuration);

        It should_be_able_to_get_all_tenants = () => tenants.GetTenants().Count().ShouldEqual(2);

        It should_be_able_to_get_first_tenant = () => tenants.GetTenants().First().ShouldEqual("tenant1");

        It should_be_able_to_get_second_tenant = () => tenants.GetTenants().Last().ShouldEqual("tenant2");

        static ConfigurationMock configuration;
        static Tenants tenants;
    }
}
