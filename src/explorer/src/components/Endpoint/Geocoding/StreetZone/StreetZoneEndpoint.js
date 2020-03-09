import React from 'react';
import Endpoint from '../../Endpoint';
import StreetZoneApi from './StreetZoneApi';
import StreetZoneDocs from './StreetZoneDocs';

export default function StreetZone(props) {
  return (
    <Endpoint collapsed={props.collapsed} name="Street and Zone" description="Finding a spatial coordinate on the ground for an address">
      <StreetZoneApi key="api"></StreetZoneApi>
      <StreetZoneDocs key="docs"></StreetZoneDocs>
    </Endpoint>
  );
}
