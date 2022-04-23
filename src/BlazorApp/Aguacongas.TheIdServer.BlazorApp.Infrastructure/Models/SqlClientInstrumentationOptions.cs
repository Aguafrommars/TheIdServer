namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class SqlClientInstrumentationOptions
    {
        //
        // Summary:
        //     Gets or sets a value indicating whether or not the OpenTelemetry.Instrumentation.SqlClient.SqlClientInstrumentation
        //     should add the names of System.Data.CommandType.StoredProcedure commands as the
        //     OpenTelemetry.Trace.SemanticConventions.AttributeDbStatement tag. Default value:
        //     True.
        public bool SetDbStatementForStoredProcedure { get; set; } = true;


        //
        // Summary:
        //     Gets or sets a value indicating whether or not the OpenTelemetry.Instrumentation.SqlClient.SqlClientInstrumentation
        //     should add the text of System.Data.CommandType.Text commands as the OpenTelemetry.Trace.SemanticConventions.AttributeDbStatement
        //     tag. Default value: False.
        public bool SetDbStatementForText { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether or not the OpenTelemetry.Instrumentation.SqlClient.SqlClientInstrumentation
        //     should parse the DataSource on a SqlConnection into server name, instance name,
        //     and/or port connection-level attribute tags. Default value: False.
        //
        // Remarks:
        //     The default behavior is to set the SqlConnection DataSource as the OpenTelemetry.Trace.SemanticConventions.AttributePeerService
        //     tag. If enabled, SqlConnection DataSource will be parsed and the server name
        //     will be sent as the OpenTelemetry.Trace.SemanticConventions.AttributeNetPeerName
        //     or OpenTelemetry.Trace.SemanticConventions.AttributeNetPeerIp tag, the instance
        //     name will be sent as the OpenTelemetry.Trace.SemanticConventions.AttributeDbMsSqlInstanceName
        //     tag, and the port will be sent as the OpenTelemetry.Trace.SemanticConventions.AttributeNetPeerPort
        //     tag if it is not 1433 (the default port).
        public bool EnableConnectionLevelAttributes { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the exception will be recorded as ActivityEvent
        //     or not. Default value: False.
        //
        // Remarks:
        //     https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/exceptions.md.
        public bool RecordException { get; set; }

    }
}