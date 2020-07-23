import setuptools
from setuptools import setup

setup(
    name='azure-functions-sql-binding',
    version='1.0.0',
    packages=['azure.functions_extensions.sql'],
    license='MIT License',
    author='Microsoft Corporation',
    url='https://github.com/Azure/azure-functions-sql-extension',
    install_requires=['azure-functions>=1.0.5'],
    classifiers=[
        'Development Status :: 3 - Alpha',
        'Programming Language :: Python :: 3',
        'Operating System :: OS Independent',
        'License :: OSI Approved :: MIT License',
    ]
)