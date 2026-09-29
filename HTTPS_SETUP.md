# HTTPS deployment checklist

The API supports HTTPS redirection and HSTS, but enforcement stays disabled until
the production certificate is installed. This prevents an outage while IIS has no
HTTPS binding.

## IIS setup

1. Point `taskflow-pal.xyz` and `www.taskflow-pal.xyz` to the server public IP.
2. Install an SSL certificate that covers both host names.
3. Add an IIS HTTPS binding on port `443` for each host name and select the certificate.
4. Keep or add the HTTP binding on port `80` so it can redirect to HTTPS.
5. Set these environment variables for the application pool:

   ```text
   Https__Enforce=true
   Https__Port=443
   ```

6. Restart the application pool.
7. Change the frontend API base URL to `https://taskflow-pal.xyz` and deploy the
   frontend over HTTPS too.

## Verification

- `http://taskflow-pal.xyz` returns a redirect to `https://taskflow-pal.xyz`.
- `https://taskflow-pal.xyz` has a valid certificate with no browser warning.
- HTTPS responses include the `Strict-Transport-Security` header in production.
- Browser API requests use HTTPS directly. Redirected CORS preflight requests can
  fail, so the frontend must not call the HTTP API URL.

Do not enable HSTS before HTTPS works correctly for the domain and its subdomains.
