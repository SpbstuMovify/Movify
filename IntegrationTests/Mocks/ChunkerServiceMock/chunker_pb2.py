# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# NO CHECKED-IN PROTOBUF GENCODE
# source: chunker.proto
# Protobuf Python Version: 5.29.0
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import runtime_version as _runtime_version
from google.protobuf import symbol_database as _symbol_database
from google.protobuf.internal import builder as _builder
_runtime_version.ValidateProtobufRuntimeVersion(
    _runtime_version.Domain.PUBLIC,
    5,
    29,
    0,
    '',
    'chunker.proto'
)
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


from google.protobuf import empty_pb2 as google_dot_protobuf_dot_empty__pb2


DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\rchunker.proto\x12\x07\x63hunker\x1a\x1bgoogle/protobuf/empty.proto\"G\n\x13ProcessVideoRequest\x12\x12\n\nbucketName\x18\x01 \x01(\t\x12\x0b\n\x03key\x18\x02 \x01(\t\x12\x0f\n\x07\x62\x61seUrl\x18\x03 \x01(\t\"1\n\x1c\x43\x61ncelVideoProcessingRequest\x12\x11\n\ttokenGuid\x18\x01 \x01(\t2\xae\x01\n\x0e\x43hunkerService\x12\x44\n\x0cProcessVideo\x12\x1c.chunker.ProcessVideoRequest\x1a\x16.google.protobuf.Empty\x12V\n\x15\x43\x61ncelVideoProcessing\x12%.chunker.CancelVideoProcessingRequest\x1a\x16.google.protobuf.EmptyB\t\xaa\x02\x06Movifyb\x06proto3')

_globals = globals()
_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, _globals)
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'chunker_pb2', _globals)
if not _descriptor._USE_C_DESCRIPTORS:
  _globals['DESCRIPTOR']._loaded_options = None
  _globals['DESCRIPTOR']._serialized_options = b'\252\002\006Movify'
  _globals['_PROCESSVIDEOREQUEST']._serialized_start=55
  _globals['_PROCESSVIDEOREQUEST']._serialized_end=126
  _globals['_CANCELVIDEOPROCESSINGREQUEST']._serialized_start=128
  _globals['_CANCELVIDEOPROCESSINGREQUEST']._serialized_end=177
  _globals['_CHUNKERSERVICE']._serialized_start=180
  _globals['_CHUNKERSERVICE']._serialized_end=354
# @@protoc_insertion_point(module_scope)
