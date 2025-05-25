# Security Policy

## Supported Versions

We aim to support the latest major version of the training materials and application. Security fixes will be provided for the most recent release.

| Version | Supported          |
| ------- | ----------------- |
| 1.x     | :white_check_mark: |
| < 1.0   | :x:               |

## Reporting a Vulnerability

If you discover a security vulnerability, please report it privately.

- **Do not create public GitHub issues for security problems.**
- Email: security@yourdomain.com (replace with your actual contact)
- Include a detailed description and steps to reproduce
- We will respond as quickly as possible and coordinate a fix

## Responsible Disclosure

We appreciate responsible disclosure and will credit reporters in release notes if desired.

## Security Best Practices

- Do not use the training application in production environments
- The provided Docker Compose disables security for ease of training (xpack.security.enabled=false)
- For real-world use, always enable authentication and TLS
- Never expose Elasticsearch or Kibana directly to the public internet

## Resources

- [Elasticsearch Security Documentation](https://www.elastic.co/guide/en/elasticsearch/reference/current/security-settings.html)
- [MIT License](LICENSE)
