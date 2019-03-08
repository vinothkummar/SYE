using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using GDSHelpers.Models.SubmissionSchema;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using SYE.Repository;
using SYE.Services;
using Xunit;

namespace SYE.Tests.Services
{
    /// <summary>
    /// This class tests the submission service is talking to the repository correctl
    /// To achieve this the repository needs to be mocked with faked return data
    /// </summary>
    public class SubmissionServiceTests
    {
        [Fact]
        public async void CreateAsyncTest()
        {
            const string id = "123";
            //arrange
            var mockedRepo = new Mock<IGenericRepository<SubmissionVM>>();
            var sut = new SubmissionService(mockedRepo.Object);

            mockedRepo.Setup(x => x.CreateAsync(It.IsAny<SubmissionVM>())).ReturnsAsync(new Document { Id = id });
            //act
            var result = await sut.CreateAsync(new SubmissionVM { Id = id });
            //assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
        }

        [Fact]
        public void DeleteAsyncTest()
        {
            const string id = "123";
            //arrange
            var mockedRepo = new Mock<IGenericRepository<SubmissionVM>>();
            var sut = new SubmissionService(mockedRepo.Object);

            mockedRepo.Setup(x => x.DeleteAsync(It.IsAny<string>()));
            // Act
            Action action = () => sut.DeleteAsync(id);
            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public async void GetByIdAsyncTest()
        {
            const string id = "123";
            //arrange
            var mockedRepo = new Mock<IGenericRepository<SubmissionVM>>();
            var sut = new SubmissionService(mockedRepo.Object);

            var submissionVm = new SubmissionVM { Id = id };
            var doc = new DocumentResponse<SubmissionVM>(submissionVm);
            mockedRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(doc);
            //act
            var result = await sut.GetByIdAsync(id);
            //assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);

        }

        [Fact]
        public async void FindByAsyncTest()
        {
            const string id = "123";
            //arrange
            var mockedRepo = new Mock<IGenericRepository<SubmissionVM>>();
            var sut = new SubmissionService(mockedRepo.Object);

            var submissionVm = new SubmissionVM {Id = id};
            var query = new List<SubmissionVM> { submissionVm }.AsQueryable();
            mockedRepo.Setup(x => x.FindByAsync(m => m.Id == id)).ReturnsAsync(query);
            //act
            var result = await sut.FindByAsync(m => m.Id == id);
            //assert
            var submissionVms = result as SubmissionVM[] ?? result.ToArray();
            submissionVms.ToList().Should().NotBeNull();
            submissionVms.Count().Should().Be(1);
            submissionVms.ToList()[0].Id.Should().Be(id);
        }

        [Fact]
        public void UpdateAsyncTest()
        {
            const string id = "123";
            //arrange
            var mockedRepo = new Mock<IGenericRepository<SubmissionVM>>();
            var sut = new SubmissionService(mockedRepo.Object);

            var submissionVm = new SubmissionVM { Id = id };
            var doc = new Document();
            mockedRepo.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubmissionVM>())).ReturnsAsync(doc);
            // Act
            Action action = () => sut.UpdateAsync(id, submissionVm);
            // Assert
            action.Should().NotThrow<Exception>();
        }
    }
}
