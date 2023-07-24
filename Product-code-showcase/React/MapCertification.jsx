import React, { useState } from "react";
import {
  Dropdown,
  DropdownToggle,
  DropdownItem,
  DropdownMenu,
} from "reactstrap";
import * as dateFormatter from "../../utils/dateFormater";
import { SlOptionsVertical } from "react-icons/sl";
import { FaUserEdit, FaTrashAlt, FaRegFileAlt } from "react-icons/fa";
import PropTypes from "prop-types";

function MappingCertification(props) {
  const aCertification = props.certification;

  const [showDropdown, setShowDropdown] = useState(false);

  const toggleModal = () => {
    setShowDropdown(!showDropdown);
  };

  const onLocalEdit = () => {
    props.onEdit(aCertification);
  };

  const onLocalDelete = () => {
    props.onDelete(aCertification);
  };

  const onLocalResults = () => {
    props.onResult(aCertification);
  }

  return (
    <tbody role="rowgroup">
      <tr role="row" className="text-center align-middle">
        <td role="cell">
          <span className="my-2">
            {aCertification.id}
          </span>
        </td>

        <td role="cell">
          <img
            src={aCertification.season?.conference?.logo}
            alt={aCertification.season?.conference?.code}
            className="avatar avatar-md"
            style={{ objectFit: "contain" }}
          />
        </td>

        <td role="cell">
          <span className='my-2'>
            {aCertification.name}
          </span>
        </td>

        <td role="cell">
          <span className="my-2">
            {aCertification.season.year}
          </span>
        </td>

        <td role="cell">
          <span className="my-2">
            {`${aCertification.isPhysicalRequired ? "Yes" : "No"}`}
          </span>
        </td>

        <td role="cell">
          <span className="my-2">
            {`${aCertification.isBackgroundCheckRequired ? "Yes" : "No"}`}
          </span>
        </td>

        <td role="cell">
          <span className="my-2">
            {`${aCertification.isTestRequired ? "Yes" : "No"}`}
          </span>
        </td>

        <td role="cell">
          <span className="my-2">
            {`${aCertification.isFitnessTestRequired ? "Yes" : "No"}`}
          </span>
        </td>

        <td role="cell">
          <span className="my-2">
            {`${aCertification.isClinicRequired ? "Yes" : "No"}`}
          </span>
        </td>

        <td role="cell">
          <span className="my-2">
            {dateFormatter.formatDateShort(aCertification.dueDate)}
          </span>
        </td>

        <td role="cell">
          <div className="my-2 cursor-pointer">
            <Dropdown isOpen={showDropdown} toggle={toggleModal}>
              <DropdownToggle tag="span">
                <SlOptionsVertical title="More Options" />
              </DropdownToggle>
              <DropdownMenu className="opacity-100">
                <DropdownItem onClick={onLocalResults}>
                  <FaRegFileAlt title="Edit" className="text-success mx-3" />
                  Results
                </DropdownItem>
                <DropdownItem onClick={onLocalEdit}>
                  <FaUserEdit title="Edit" className="text-warning mx-3" />
                  Edit
                </DropdownItem>
                <DropdownItem onClick={onLocalDelete}>
                  <FaTrashAlt title="Delete" className="text-danger mx-3" />
                  Delete
                </DropdownItem>
              </DropdownMenu>
            </Dropdown>
          </div>
        </td>
      </tr>
    </tbody>
  );
}

MappingCertification.propTypes = {
  certification: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    season: PropTypes.shape({
      year: PropTypes.number.isRequired,
      conference: PropTypes.shape({
        code: PropTypes.string.isRequired,
        logo: PropTypes.string.isRequired
      }).isRequired
    }).isRequired,
    isPhysicalRequired: PropTypes.bool.isRequired,
    isBackgroundCheckRequired: PropTypes.bool.isRequired,
    isTestRequired: PropTypes.bool.isRequired,
    isFitnessTestRequired: PropTypes.bool.isRequired,
    isClinicRequired: PropTypes.bool.isRequired,
    dueDate: PropTypes.string.isRequired
  }).isRequired,
  onEdit: PropTypes.func.isRequired,
  onDelete: PropTypes.func.isRequired,
  onResult: PropTypes.func.isRequired
}

export default MappingCertification;
