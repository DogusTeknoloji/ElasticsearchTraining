# Contributing to Elasticsearch Training Materials

First off, thank you for considering contributing to our Elasticsearch training materials! 🎉

## How Can You Contribute?

### 🐛 Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates. When creating a bug report, include:

- **Clear description** of the issue
- **Steps to reproduce** the behavior
- **Expected behavior** vs actual behavior
- **Environment details** (OS, .NET version, Elasticsearch version)
- **Screenshots** if applicable

### 💡 Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear and descriptive title**
- **Provide detailed description** of the suggested enhancement
- **Explain why this enhancement would be useful** to training participants
- **Consider the scope** - should it be part of the core training or an advanced topic?

### 📝 Content Contributions

#### Training Materials

- **Language consistency**: Turkish for explanations, English for technical terms and code
- **Practical focus**: Include real-world scenarios and examples
- **Hands-on approach**: Every concept should have practical exercises
- **Log management emphasis**: Priority on log analysis use cases

#### Code Contributions

- **Follow .NET conventions** and existing code style
- **Add tests** for new functionality
- **Update documentation** when necessary
- **Ensure compatibility** with the target training environment

## 🚀 Getting Started

### Development Setup

1. **Fork the repository**
2. **Clone your fork** locally
3. **Set up the development environment**:
   ```bash
   cd ElasticsearchTraining
   cd docker && docker-compose up -d
   cd ../src/ElasticTraining && dotnet restore
   ```
4. **Create a branch** for your changes

### Making Changes

1. **Follow the existing project structure**
2. **Test your changes** thoroughly
3. **Update documentation** if needed
4. **Follow commit message conventions**

### Pull Request Process

1. **Update the README.md** if your changes affect setup or usage
2. **Update the CHANGELOG.md** with your changes
3. **Ensure all tests pass** and code builds successfully
4. **Request review** from maintainers

## 📚 Training Content Guidelines

### Content Structure

- **Modular approach**: Each section should be self-contained
- **Progressive complexity**: Start simple, build complexity gradually
- **Practical examples**: Real-world scenarios over theoretical examples
- **Consistent formatting**: Follow established markdown conventions

### Technical Standards

- **Elasticsearch version**: Target 8.19.2 or compatible versions (8.x and 9.x)
- **Code examples**: Working, tested examples only
- **.NET compatibility**: Ensure compatibility with .NET 9.0
- **Docker support**: All examples should work with the provided Docker setup
- **NEST client**: Currently using NEST 7.17.5 (note: EOL late 2025)

### Language Guidelines

- **Primary language**: Turkish for instructions and explanations
- **Technical terms**: English for all Elasticsearch terminology, API calls, field names
- **Code comments**: English for code comments and technical documentation
- **Consistency**: Maintain the established tone and style

## 🎯 Areas Needing Contributions

### High Priority

- [ ] Additional practical exercises for each training module
- [ ] More realistic log generation scenarios
- [ ] Advanced aggregation examples
- [ ] Performance optimization examples
- [ ] Troubleshooting guides

### Medium Priority

- [ ] Video tutorial supplements
- [ ] Interactive online exercises
- [ ] Automated testing for training examples
- [ ] Multi-language support (English version)
- [ ] Advanced topics (security, clustering)

### Community Contributions

- [ ] Translation to other languages
- [ ] Integration with other logging frameworks
- [ ] Cloud deployment guides (AWS, Azure, GCP)
- [ ] Alternative technology stacks (Python, Java examples)

## 📋 Style Guides

### Markdown

- Use proper heading hierarchy
- Include code syntax highlighting
- Add alt text for images
- Use relative links for internal references

### Code Style

- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Include XML documentation for public APIs
- Write unit tests for complex logic

### Git Commit Messages

```text
type(scope): description

- feat: new feature
- fix: bug fix
- docs: documentation changes
- style: formatting changes
- refactor: code refactoring
- test: adding tests
- chore: maintenance tasks
```

## 🤝 Community

### Code of Conduct

This project adheres to a code of conduct. By participating, you are expected to uphold this code.

### Getting Help

- **Documentation**: Start with the README and training materials
- **Issues**: Search existing issues before creating new ones
- **Discussions**: Use GitHub Discussions for questions and ideas

### Recognition

Contributors will be acknowledged in:

- README.md contributors section
- Training material credits
- Release notes

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for helping make Elasticsearch training accessible and effective! 🙏
