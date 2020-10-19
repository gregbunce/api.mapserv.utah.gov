using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Models.ArcGis;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Serilog;
using Xunit;

namespace AGRC.api.Features.Milepost {
    public class DominantRouteResolverTests {
        public class ComputationTests {
            [Fact]
            public async Task Should_build_route_map_for_every_location() {
                var locations = new[] {
                    new GeometryToMeasure.ResponseLocation {
                        RouteId = "1",
                        Measure = 0
                    },
                    new GeometryToMeasure.ResponseLocation {
                        RouteId = "2",
                        Measure = 0
                    },
                    new GeometryToMeasure.ResponseLocation {
                        RouteId = "3",
                        Measure = 0
                    }
                };

                var response = new Concurrencies.ResponseContract {
                    Locations = new[] {
                        new Concurrencies.ResponseLocations {
                            RouteId = "1",
                            FromMeasure = -1,
                            ToMeasure = 1,
                            Concurrencies = new [] {
                                new Concurrencies.ConcurrencyLocations {
                                    RouteId = "1",
                                    FromMeasure = -1,
                                    ToMeasure = 1,
                                    IsDominant = true
                                }
                            }
                        },
                        new Concurrencies.ResponseLocations {
                            RouteId = "2",
                            FromMeasure = -1,
                            ToMeasure = 1,
                            Concurrencies = new Concurrencies.ConcurrencyLocations[0]
                        },
                        new Concurrencies.ResponseLocations {
                            RouteId = "3",
                            FromMeasure = -1,
                            ToMeasure = 1,
                            Concurrencies = new Concurrencies.ConcurrencyLocations[0]
                        }
                    }
                };

                var computation = new DominantRouteResolver.Computation(locations, null, 0);

                var httpHandler = new Mock<TestingHttpMessageHandler>{ CallBase = true };

                httpHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(response))
                }).Verifiable();

                var client = new HttpClient(httpHandler.Object) {
                    BaseAddress = new Uri("https://testing.me")
                };

                var factory = new Mock<IHttpClientFactory>();
                factory.Setup(x => x.CreateClient("udot")).Returns(client);

                var log = new Mock<ILogger> { DefaultValue = DefaultValue.Mock };
                var handler = new DominantRouteResolver.Handler(factory.Object, new PythagoreanDistance(), log.Object);

                var result = await handler.Handle(computation, default);

                httpHandler.Verify(x => x.Send(It.IsAny<HttpRequestMessage>()), Times.Exactly(1));
            }

            [Fact]
            public void Should_create_request_with_measures_for_every_location() {

            }
        }

        public class DominantRouteDescriptorComparerTests {
            [Fact]
            public void Should_order_dominant_then_distance() {

            }
        }

        public class TestingHttpMessageHandler : HttpMessageHandler {
            public virtual HttpResponseMessage Send(HttpRequestMessage request) =>
                throw new NotImplementedException("Now we can setup this method with our mocking framework");

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken) =>
                    Task.FromResult(Send(request));
        }
    }
}