using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using api.mapserv.utah.gov.Models;
using MediatR;
using Moq;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Cache;
using Serilog;
using System.Threading;

namespace benchmarks.tests {
    [RPlotExporter, CategoriesColumn, RankColumn, MeanColumn, MedianColumn, MemoryDiagnoser]
    public class AddressParsingBenchMark {
        private readonly IRequestHandler<AddressParsing.Command, CleansedAddress> _handler;
        private readonly AddressParsing.Command _po_box_request;
        private readonly AddressParsing.Command _unit_with_no_type;

        public AddressParsingBenchMark() {
            var abbreviations = new Abbreviations();
            var regex = new RegexCache(abbreviations);

            _handler = new AddressParsing.Handler(regex, abbreviations, new Mock<ILogger>().Object);

            _po_box_request = new AddressParsing.Command("P O Box 123");
            _unit_with_no_type = new AddressParsing.Command("625 NORTH REDWOOD ROAD 6");
        }

        [Benchmark]
        public async Task<CleansedAddress> PoBoxes () => await _handler.Handle(_po_box_request, CancellationToken.None);

        [Benchmark]
        public async Task<CleansedAddress> UnitsWithNoTypes () => await _handler.Handle(_unit_with_no_type, CancellationToken.None);

        }
}
