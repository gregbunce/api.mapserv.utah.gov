import React from 'react';
// import './EndpointSwitch.css';

import Toggle from 'react-toggle';
import { TypeLabel, RequiredLabel } from './EndpointLabel';

export default function Field(props) {
  return (
    <div className="flex flex-wrap w-full pt-3 md:flex-no-wrap items-center">
      <label className="self-center mx-2 md:ml-4 pb-2 md:pb-2 text-sm leading-5 font-medium text-gray-700 lg:w-1/4 md:w-1/2">{props.name}</label>
      <span className="w-full">
        <Toggle
          onChange={event =>
            props.dispatch({
              type: props.name,
              payload: event.target.checked
            })
          }
          defaultChecked={false}
        />
      </span>
      <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
        <TypeLabel type={props.type} />
        <RequiredLabel required={props.required} />
      </div>
    </div>
  );
}