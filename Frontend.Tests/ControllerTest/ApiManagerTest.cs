﻿using Moq;
using Moq.Protected;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Frontend.Controller;
using System.Diagnostics;
using System.Net;

namespace Frontend.Tests.ControllerTest
{
    public class ApiManagerTest
    {
        [Fact]
        public async Task RetrieveRandomPuzzleTest()
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            HttpResponseMessage httpResponseMessage = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonContent.Create(new
                {
                    id = "1",
                    puzzleId = "00008",
                    fen = "r6k/pp2r2p/4Rp1Q/3p4/8/1N1P2R1/PqP2bPP/7K b - - 0 24",
                    moves = "f2g3 e6e7 b2b1 b3c1 b1c1 h6c1",
                    rating = 1853,
                    themes = "crushing hangingPiece long middlegame"
                }
                    )
            };
            //httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<PuzzleDTO>()).ReturnsAsync(httpResponseMessage);
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new System.Uri("http://localhost:52474/")
            };

            ApiManager apm = new(httpClient);

            var result = await apm.RetrieveRandomPuzzle();
            Assert.Equivalent(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<PuzzleDTO>(result);
            var expectedResult = new PuzzleDTO("1", "00008", "r6k/pp2r2p/4Rp1Q/3p4/8/1N1P2R1/PqP2bPP/7K b - - 0 24", "f2g3 e6e7 b2b1 b3c1 b1c1 h6c1", 1853, "crushing hangingPiece long middlegame");
            Assert.Equivalent(expectedResult, result);
        }
    }
}
