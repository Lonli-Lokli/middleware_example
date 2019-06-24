# middleware_example

Any requet with 'Bearer' string in Authorization header is treated is valid and authorized with Admin rights.

Goal: make an internal inmemory request without auth pipeline to avoid passing secret headers.
